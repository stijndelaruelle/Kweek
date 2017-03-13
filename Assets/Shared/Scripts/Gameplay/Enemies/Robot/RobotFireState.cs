using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyWeaponPickupBehaviour))]
public class RobotFireState : IAbstractTargetState
{
    private EnemyWeaponPickupBehaviour m_Robot;

    [Header("Fireing")]
    [Space(5)]
    [SerializeField]
    private Transform m_FirePosition;

    [SerializeField]
    private float m_FireDelay;
    private float m_FireDelayTimer = 0.0f;

    [SerializeField]
    private float m_MaxShootRange = 5.0f;

    [SerializeField]
    private float m_ChargeTime = 1.0f;

    [Space(10)]
    [Header("Chest & Hand rotation")]
    [Space(5)]

    [Tooltip("Degrees per second")]
    [SerializeField]
    private float m_ChestRotationSpeed;

    [SerializeField]
    private float m_HorizontalChestRotationOffset;

    [SerializeField]
    private float m_VerticalChestRotationOffset;

    [Space(10)]
    [Header("Scanning")]
    [Space(5)]
    [SerializeField]
    private Transform m_ViewPosition;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private BasicSearchChaseState m_ChaseState;

    private bool m_IsInFireStance = false;
    private bool m_IsSwitchingOut = false;

    private Vector3 m_Target;
    private Coroutine m_ChargeRoutine;
    private Quaternion m_LastChestLocalRotation; //Chest bone rotation constatntly resets, cache it here.
    private Quaternion m_LastRightArmLocalRotation;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Robot = GetComponent<EnemyWeaponPickupBehaviour>();
    }

    public override void Enter()
    {
        Debug.Log("Entered fire state!");

        m_Robot.TriggerStayEvent += OnStateTriggerStay;
        m_Robot.TriggerExitEvent += OnStateTriggerExit;
        m_Robot.AnimatorIKEvent += OnStateAnimatorIK;

        //Stop the character from moving, both gamewise as visually
        m_Robot.NavMeshAgent.Stop();

        m_Target = Vector3.zero;
        m_FireDelayTimer = m_FireDelay;
        m_IsInFireStance = false;
        m_IsSwitchingOut = false;

        m_LastChestLocalRotation = Quaternion.identity;
    }

    public override void Exit()
    {
        if (m_Robot == null)
            return;

        m_Robot.TriggerStayEvent -= OnStateTriggerStay;
        m_Robot.TriggerExitEvent -= OnStateTriggerExit;
        m_Robot.AnimatorIKEvent -= OnStateAnimatorIK;

        if (m_ChargeRoutine != null)
        {
            StopWeaponCharging();
            StopCoroutine(m_ChargeRoutine);
        }
    }

    public override void StateUpdate()
    {
        HandleSwitchIn();
        HandleSwitchOut();

        HandleShooting();
    }

    //Shooting
    private void HandleShooting()
    {
        if (m_IsSwitchingOut || m_FireDelayTimer > 0.0f)
            return;

        if (m_ChargeRoutine != null)
            return;

        //Check the distance between us and the target
        Vector3 diff = m_Target - m_FirePosition.position;
        float distance = diff.magnitude;

        //If we are too far away, chase to the position!
        if (distance > m_MaxShootRange)
        {
            SwitchOut();
        }

        m_ChargeRoutine = StartCoroutine(ChargeRoutine(m_ChargeTime));
    }

    private IEnumerator ChargeRoutine(float chargeTime)
    {
        StartWeaponCharging();

        yield return new WaitForSeconds(chargeTime);

        StopWeaponCharging();

        m_ChargeRoutine = null;
    }

    private void StartWeaponCharging()
    {
        Ray fireRay = new Ray(m_FirePosition.position, m_FirePosition.forward);
        m_Robot.Weapon.Use(fireRay);
    }

    private void StopWeaponCharging()
    {
        Ray fireRay = new Ray(m_FirePosition.position, m_FirePosition.forward);
        bool success = m_Robot.Weapon.StopUse(fireRay);

        //TODO: If this is not a success try stop using it over and over again (?)
    }

    //Switching
    private void HandleSwitchIn()
    {
        if (m_IsSwitchingOut)
            return;

        if (m_FireDelayTimer > 0.0f)
        {
            m_FireDelayTimer -= Time.deltaTime;

            //If we are almost ready to fire, call the animation already once. Otherwise we fire our first bullet into the ground
            if (m_IsInFireStance == false && m_FireDelayTimer <= 0.2f) //hardcoded value from the animator
            {
                m_Robot.Animator.SetTrigger("ReadyFireTrigger");
                m_IsInFireStance = true;
            }
        }
    }

    private void HandleSwitchOut()
    {
        if (!m_IsSwitchingOut)
            return;

        m_FireDelayTimer += Time.deltaTime;
        if (m_FireDelayTimer >= m_FireDelay)
        {
            m_ChaseState.SetTarget(m_Target);
            m_Robot.SwitchState(m_ChaseState);
        }
    }

    private void SwitchOut()
    {
        if (m_IsSwitchingOut)
            return;

        m_FireDelayTimer = 0.0f;
        m_IsSwitchingOut = true;
        m_Robot.Animator.SetTrigger("MovementTrigger");
    }

    private void OnStateTriggerStay(Collider other)
    {
        //Check if it's the player
        if (other.bounds.center == m_Target)
        {
            //If so check if he's within the specified angle
            Vector3 diffPos = other.transform.position - m_Robot.transform.position;
            float dot = Vector3.Dot(m_Robot.transform.forward, diffPos.normalized);
            float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

            if (degAngle <= m_ViewAngle)
            {
                //If so, check line of sight
                Vector3 middleTop = other.bounds.center;
                middleTop.y += other.bounds.extents.y * 0.5f;

                Ray ray = new Ray(m_ViewPosition.position, (middleTop - m_ViewPosition.position));

                RaycastHit hitInfo;
                bool success = Physics.Raycast(ray, out hitInfo);

                if (!(success && hitInfo.collider == other))
                {
                    //Stop firing
                    SwitchOut();
                }
                else
                {
                    //Stop switching out!
                    m_IsSwitchingOut = false;
                }
            }
            else
            {
                //If not, stop firing
                SwitchOut();
            }
        }
    }

    public void OnStateTriggerExit(Collider other)
    {
        if (other.bounds.center == m_Target)
        {
            //Change to the chasing state
            SwitchOut();
        }
    }

    private void OnStateAnimatorIK(int layerIndex)
    {
        if (layerIndex == 1)
        {
            //Rotate the head
            float normTimer = (m_FireDelay - m_FireDelayTimer) / m_FireDelay;
            m_Robot.Animator.SetLookAtWeight(normTimer);
            m_Robot.Animator.SetLookAtPosition(m_Target);

            //Rotate the chest
            if (m_ChestRotationSpeed > 0)
            {
                Quaternion desiredChestRotation = CalculateLocalBoneRotation(HumanBodyBones.Chest, m_HorizontalChestRotationOffset, m_VerticalChestRotationOffset);

                if (m_LastChestLocalRotation == Quaternion.identity)
                {
                    m_LastChestLocalRotation = Quaternion.RotateTowards(m_Robot.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation,
                                                                        desiredChestRotation,
                                                                        m_ChestRotationSpeed * Time.deltaTime);
                }
                else
                {
                    m_LastChestLocalRotation = Quaternion.RotateTowards(m_LastChestLocalRotation,
                                                                        desiredChestRotation,
                                                                        m_ChestRotationSpeed * Time.deltaTime);
                }

                m_Robot.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, m_LastChestLocalRotation);
            }
        }
    }

    private Quaternion CalculateLocalBoneRotation(HumanBodyBones boneType, float horizOffset, float vertOffset)
    {
        Quaternion desiredRotation;

        if (m_IsSwitchingOut == false)
        {
            Vector3 direction = (m_Target - m_Robot.Animator.GetBoneTransform(boneType).position).normalized;
            desiredRotation = Quaternion.LookRotation(direction);
            Vector3 euler = desiredRotation.eulerAngles;

            //Add the transform of the character, otherwise things get weird.
            euler.z = 360.0f - euler.x + (m_Robot.transform.rotation.eulerAngles.x) + vertOffset;
            euler.x = 360.0f - euler.y + (m_Robot.transform.rotation.eulerAngles.y) + horizOffset;
            euler.y = 0.0f;

            desiredRotation = Quaternion.Euler(euler);
        }
        else
        {
            desiredRotation = m_Robot.Animator.GetBoneTransform(boneType).localRotation;
        }

        return desiredRotation;
    }

    public override void SetTarget(Vector3 target)
    {
        m_Target = target;
    }

    public override string ToString()
    {
        return "Firing";
    }
}