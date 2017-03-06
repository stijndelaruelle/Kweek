using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ZombieBehaviour))]
public class ZombieAttackState : IAbstractState
{
    private ZombieBehaviour m_Zombie;

    [SerializeField]
    private ZombieChaseState m_ChaseState;

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
    }

    public override void Exit()
    {
    }

    public override void StateUpdate()
    {
        HandleAttacking();
    }

    private void HandleAttacking()
    {
        m_Zombie.Weapon.Use();
    }

    public override string ToString()
    {
        return "Attacking";
    }
}