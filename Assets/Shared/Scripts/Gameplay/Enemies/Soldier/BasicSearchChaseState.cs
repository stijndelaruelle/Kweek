using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBehaviour))]
public class BasicSearchChaseState : IAbstractState
{
    private EnemyBehaviour m_Soldier;

    [Header("Movement")]
    [Space(5)]
    [SerializeField]
    private float m_MovementSpeed;
    private Vector3 m_TargetPosition;

    [SerializeField]
    private float m_MinChaseTime = 1.0f; //Fixes some back and forth state switching.
    private float m_ChaseTimer;

    [Space(10)]
    [Header("Scanning")]
    [Space(5)]
    [SerializeField]
    private Transform m_ViewPosition;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private BasicPatrolState m_PatrolState;

    [SerializeField]
    private IAbstractTargetState m_FireState;


    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Soldier = GetComponent<EnemyBehaviour>();
    }

    public override void Enter()
    {
        Debug.Log("Entered Chase state!");
        
        m_Soldier.TriggerStayEvent += OnStateTriggerStay;

        m_Soldier.NavMeshAgent.Resume();
        m_Soldier.NavMeshAgent.speed = m_MovementSpeed;
        m_Soldier.NavMeshAgent.destination = m_TargetPosition;

        m_Soldier.Animator.enabled = true;
        m_Soldier.Animator.SetTrigger("MovementTrigger");

        m_ChaseTimer = 0.0f;
    }

    public override void Exit()
    {
        if (m_Soldier == null)
            return;

        m_Soldier.TriggerStayEvent -= OnStateTriggerStay;
    }

    public override void StateUpdate()
    {
        HandleMovement();

        m_ChaseTimer += Time.deltaTime;
    }

    private void HandleMovement()
    {
        NavMeshAgent agent = m_Soldier.NavMeshAgent;
        Animator animator = m_Soldier.Animator;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            //If we still didn't switch to the fire state at this point, we lost the player. Go back to patrolling
            m_Soldier.SwitchState(m_PatrolState);
        }
    }

    private void OnStateTriggerStay(Collider other)
    {
        //Check if it's the player
        if (other.tag == "Player")
        {
            //If so check if he's within the specified angle
            Vector3 diffPos = other.transform.position - m_Soldier.transform.position;
            float dot = Vector3.Dot(m_Soldier.transform.forward, diffPos.normalized);
            float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

            if (degAngle <= m_ViewAngle)
            {
                Vector3 middleTop = other.bounds.center;
                middleTop.y += other.bounds.extents.y * 0.5f;

                Ray ray = new Ray(m_ViewPosition.position, (middleTop - m_ViewPosition.position));

                RaycastHit hitInfo;
                bool success = Physics.Raycast(ray, out hitInfo);

                if (success && hitInfo.collider == other && m_ChaseTimer >= m_MinChaseTime)
                {
                    //Change to the firing state
                    m_Soldier.SwitchState(m_FireState);
                    m_FireState.SetTarget(other.bounds.center);
                }
            }
        }
    }

    public void SetTarget(Vector3 targetPosition)
    {
        m_TargetPosition = targetPosition;
        m_Soldier.NavMeshAgent.destination = m_TargetPosition;
    }

    public override string ToString()
    {
        return "Chasing";
    }
}