using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ZombieBehaviour))]
public class ZombieAttackState : IAbstractState
{
    private ZombieBehaviour m_Zombie;

    [SerializeField]
    private float m_AttackDistance;
    public float AttackDistance
    {
        get { return m_AttackDistance; }
    }

    [Tooltip("Degrees per second")]
    [SerializeField]
    private float m_ChestRotationSpeed;
    private Quaternion m_LastChestLocalRotation; //Chest bone rotation constatntly resets, cache it here.

    [SerializeField]
    private ZombieChaseState m_ChaseState;
    private Transform m_Target;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Zombie = GetComponent<ZombieBehaviour>();
    }

    public override void Enter()
    {
        Debug.Log("Entered attacking state!");

        m_Zombie.NavMeshAgent.Stop();

        m_Zombie.Animator.enabled = true;
        m_Zombie.AnimatorIKEvent += OnStateAnimatorIK;
    }

    public override void Exit()
    {
        if (m_Zombie != null)
            m_Zombie.AnimatorIKEvent -= OnStateAnimatorIK;
    }

    public override void StateUpdate()
    {
        HandleAttacking();
        HandleStateSwitching();
    }

    private void HandleAttacking()
    {
        m_Zombie.Weapon.Use();
    }

    private void HandleStateSwitching()
    {
        float distance = (transform.position - m_Target.position).magnitude;

        if (distance > m_AttackDistance + 0.2f) //A little offset to avoid jittering
        {
            m_ChaseState.SetTarget(m_Target);
            m_Zombie.SwitchState(m_ChaseState);
        }
    }

    private void OnStateAnimatorIK(int layerIndex)
    {
        if (layerIndex == 1)
        {
            //Rotate the head
            //float normTimer = (m_FireDelay - m_FireDelayTimer) / m_FireDelay;
            m_Zombie.Animator.SetLookAtWeight(1.0f);
            m_Zombie.Animator.SetLookAtPosition(m_Target.position);

            //Rotate the chest
            if (m_ChestRotationSpeed > 0)
            {
                Quaternion desiredChestRotation = CalculateLocalBoneRotation(HumanBodyBones.Chest, 0.0f, 0.0f);

                if (m_LastChestLocalRotation == Quaternion.identity)
                {
                    m_LastChestLocalRotation = Quaternion.RotateTowards(m_Zombie.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation,
                                                                        desiredChestRotation,
                                                                        m_ChestRotationSpeed * Time.deltaTime);
                }
                else
                {
                    m_LastChestLocalRotation = Quaternion.RotateTowards(m_LastChestLocalRotation,
                                                                        desiredChestRotation,
                                                                        m_ChestRotationSpeed * Time.deltaTime);
                }

                m_Zombie.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, m_LastChestLocalRotation);
            }
        }
    }

    private Quaternion CalculateLocalBoneRotation(HumanBodyBones boneType, float horizOffset, float vertOffset)
    {
        Quaternion desiredRotation;

        Vector3 direction = (m_Target.position - m_Zombie.Animator.GetBoneTransform(boneType).position).normalized;
        desiredRotation = Quaternion.LookRotation(direction);
        Vector3 euler = desiredRotation.eulerAngles;

        //Add the transform of the character, otherwise things get weird.
        euler.z = 360.0f - euler.x + (m_Zombie.transform.rotation.eulerAngles.x) + vertOffset;
        euler.x = 360.0f - euler.y + (m_Zombie.transform.rotation.eulerAngles.y) + horizOffset;
        euler.y = 0.0f;

        return Quaternion.Euler(euler);
    }

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    public override string ToString()
    {
        return "Attacking";
    }
}