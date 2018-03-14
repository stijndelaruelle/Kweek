using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBehaviour))]
public class ZombieChaseState : IAbstractState
{
    private EnemyBehaviour m_Zombie;

    [Header("Movement")]
    [Space(5)]
    [SerializeField]
    private float m_MovementSpeed;
    private IDamageableObject m_Target;

    [Space(10)]
    [Header("Scanning")]
    [Space(5)]
    [SerializeField]
    private float m_DistanceMargin;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private ZombieWanderState m_WanderState;

    [SerializeField]
    private ZombieAttackState m_AttackState;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Zombie = GetComponent<EnemyBehaviour>();
    }

    public override void Enter()
    {
        //Debug.Log("Entered chasing state!");

        m_Zombie.NavMeshAgent.destination = transform.position;
        m_Zombie.NavMeshAgent.speed = m_MovementSpeed;

        m_Zombie.NavMeshAgent.isStopped = false;

        m_Zombie.Animator.enabled = true;
        m_Zombie.Animator.SetTrigger("MovementTrigger");
    }

    public override void Exit()
    {
    }

    public override void StateUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        NavMeshAgent agent = m_Zombie.NavMeshAgent;

        if (agent == null)
            return;

        //Check if we reached our destination
        //Find a spot a little in front of our target
        Transform targetTransform = m_Target.transform;

        Vector3 targetPos = m_Target.GetPosition() + (targetTransform.forward * m_DistanceMargin);
        m_Zombie.NavMeshAgent.destination = targetPos;

        float distance = (transform.position - targetTransform.position).magnitude;
        if (distance < m_AttackState.AttackDistance)
        {
            //Check if we are looking towards the target
            Vector3 diffPos = m_Target.GetPosition() - m_Zombie.transform.position;
            float dot = Vector3.Dot(m_Zombie.transform.forward, diffPos.normalized);
            float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

            if (degAngle <= m_ViewAngle)
            {
                m_AttackState.SetTarget(m_Target);
                m_Zombie.SwitchState(m_AttackState);
            }
        }

        //Target is dead
        if (m_Target.IsDead())
        {
            m_Zombie.SwitchState(m_WanderState);
        }
    }

    public void SetTarget(IDamageableObject target)
    {
        m_Target = target;
    }

    public override string ToString()
    {
        return "Chasing";
    }
}