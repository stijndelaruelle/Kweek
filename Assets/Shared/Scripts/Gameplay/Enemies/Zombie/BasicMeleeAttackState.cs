using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyWeaponPickupBehaviour))]
public class BasicMeleeAttackState : IAbstractTargetState
{
    private EnemyWeaponPickupBehaviour m_Behaviour;

    [Space(10)]
    [Header("Chest rotation")]
    [Space(5)]

    [Tooltip("Degrees per second")]
    [SerializeField]
    private float m_ChestRotationSpeed;
    private Quaternion m_LastChestLocalRotation; //Chest bone rotation constatntly resets, cache it here.

    [Space(10)]
    [Header("Scanning")]
    [Tooltip("Will change to 'ChaseState' when outside of this range")]
    [Space(5)]
    [SerializeField]
    private IAbstractTargetState m_ChaseState;

    [SerializeField]
    private Transform m_ViewPosition;

    [SerializeField]
    private float m_ViewRadius;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private LayerMask m_RadiusLayerMask;

    [SerializeField]
    private LayerMask m_RaycastLayerMask;

    [Space(10)]
    [Header("Other States")]
    [Space(5)]
    [SerializeField]
    private IAbstractState m_WanderState;

    private IDamageableObject m_Target;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Behaviour = GetComponent<EnemyWeaponPickupBehaviour>();
    }

    public override void Enter()
    {
        Debug.Log("Entered attacking state!");

        m_Behaviour.NavMeshAgent.isStopped = true;

        m_Behaviour.Animator.enabled = true;
        m_Behaviour.AnimatorIKEvent += OnStateAnimatorIK;
    }

    public override void Exit()
    {
        if (m_Behaviour != null)
            m_Behaviour.AnimatorIKEvent -= OnStateAnimatorIK;
    }

    public override void StateUpdate()
    {
        HandleAttacking();
        HandleScanning();
    }

    private void HandleAttacking()
    {
        m_Behaviour.Weapon.Use();
    }

    private void HandleScanning()
    {
        Collider[] colliders = Physics.OverlapSphere(m_ViewPosition.position, m_ViewRadius, m_RadiusLayerMask.value);

        //For all targets in my radius
        for (int i = 0; i < colliders.Length; ++i)
        {
            Collider other = colliders[i];

            IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
            if (damageableObject == null)
                return;

            damageableObject = damageableObject.GetMainDamageableObject();

            //Check if it's my target
            if (damageableObject == m_Target)
            {
                if (m_Target.IsDead())
                {
                    SwitchOut();
                    return;
                }

                //If so check if he's within the specified angle
                Vector3 diffPos = other.transform.position - m_Behaviour.transform.position;
                float dot = Vector3.Dot(m_Behaviour.transform.forward, diffPos.normalized);
                float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

                if (degAngle <= m_ViewAngle)
                {
                    //If so, check line of sight
                    Ray ray = new Ray(m_ViewPosition.position, (damageableObject.GetPosition() - m_ViewPosition.position) * 100);
                    //Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);
                    RaycastHit hitInfo;

                    m_Behaviour.MakeCollidersIgnoreRaycasts(true);
                    bool success = Physics.Raycast(ray, out hitInfo, m_RaycastLayerMask.value);
                    m_Behaviour.MakeCollidersIgnoreRaycasts(false);

                    /*
                    //If so, check line of sight
                    Ray ray = new Ray(m_ViewPosition.position, (damageableObject.GetPosition() - m_ViewPosition.position));
                    //Debug.DrawRay(ray.origin, ray.direction, Color.blue);

                    RaycastHit[] hitInfo = Physics.RaycastAll(ray, m_ViewRadius, m_RaycastLayerMask.value);

                    foreach (RaycastHit hit in hitInfo)
                    {
                        IDamageableObject foundDamageableObject = hit.collider.GetComponent<IDamageableObject>();
                        if (foundDamageableObject != null)
                        {
                            foundDamageableObject = foundDamageableObject.GetMainDamageableObject();

                            if (damageableObject != foundDamageableObject)
                            {
                                SwitchOut();
                            }

                            return;
                        }
                    }
                    */

                    if (success)
                    {
                        IDamageableObject foundDamageableObject = hitInfo.collider.GetComponent<IDamageableObject>();
                        if (foundDamageableObject != null)
                        {
                            foundDamageableObject = foundDamageableObject.GetMainDamageableObject();

                            if (damageableObject != foundDamageableObject)
                            {
                                SwitchOut();
                            }

                            return;
                        }
                    }
                }

                //Our target is no longer in our line of sight, start chasing
                SwitchOut();
                return;
            }
        }

        //Our target is no longer in our radius, start chasing
        SwitchOut();
    }

    private void SwitchOut()
    {
        if (m_Target.IsDead())
        {
            m_Behaviour.SwitchState(m_WanderState);
        }
        else
        {
            m_ChaseState.SetTarget(m_Target);
            m_Behaviour.SwitchState(m_ChaseState);
        }
    }

    private void OnStateAnimatorIK(int layerIndex)
    {
        if (layerIndex == 1)
        {
            //Rotate the head
            //float normTimer = (m_FireDelay - m_FireDelayTimer) / m_FireDelay;
            m_Behaviour.Animator.SetLookAtWeight(1.0f);
            m_Behaviour.Animator.SetLookAtPosition(m_Target.GetPosition());

            //Rotate the chest
            if (m_ChestRotationSpeed > 0)
            {
                Quaternion desiredChestRotation = CalculateLocalBoneRotation(HumanBodyBones.Chest, 0.0f, 0.0f);

                if (m_LastChestLocalRotation == Quaternion.identity)
                {
                    m_LastChestLocalRotation = Quaternion.RotateTowards(m_Behaviour.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation,
                                                                        desiredChestRotation,
                                                                        m_ChestRotationSpeed * Time.deltaTime);
                }
                else
                {
                    m_LastChestLocalRotation = Quaternion.RotateTowards(m_LastChestLocalRotation,
                                                                        desiredChestRotation,
                                                                        m_ChestRotationSpeed * Time.deltaTime);
                }

                m_Behaviour.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, m_LastChestLocalRotation);
            }
        }
    }

    private Quaternion CalculateLocalBoneRotation(HumanBodyBones boneType, float horizOffset, float vertOffset)
    {
        Quaternion desiredRotation;

        Vector3 direction = (m_Target.GetPosition() - m_Behaviour.Animator.GetBoneTransform(boneType).position).normalized;
        desiredRotation = Quaternion.LookRotation(direction);
        Vector3 euler = desiredRotation.eulerAngles;

        //Add the transform of the character, otherwise things get weird.
        euler.z = 360.0f - euler.x + (m_Behaviour.transform.rotation.eulerAngles.x) + vertOffset;
        euler.x = 360.0f - euler.y + (m_Behaviour.transform.rotation.eulerAngles.y) + horizOffset;
        euler.y = 0.0f;

        return Quaternion.Euler(euler);
    }

    public override void SetTarget(IDamageableObject target)
    {
        m_Target = target;
    }

    public override string ToString()
    {
        return "Attacking";
    }
}