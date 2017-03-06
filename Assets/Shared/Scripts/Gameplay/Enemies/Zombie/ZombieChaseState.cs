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
    private Transform m_Target;

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
        Debug.Log("Entered chasing state!");

        m_Zombie.NavMeshAgent.destination = transform.position;
        m_Zombie.NavMeshAgent.speed = m_MovementSpeed;

        m_Zombie.NavMeshAgent.Resume();

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
        m_Zombie.NavMeshAgent.destination = m_Target.position;
        if (agent.remainingDistance <= 0.75f)
        {
            m_Zombie.SwitchState(m_AttackState);
        }
    }

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    public override string ToString()
    {
        return "Chasing";
    }
}