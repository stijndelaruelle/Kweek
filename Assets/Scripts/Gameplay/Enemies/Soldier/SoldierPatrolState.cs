using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SoldierBehaviour))]
public class SoldierPatrolState : IAbstractState
{
    private SoldierBehaviour m_Soldier;

    [Header("Movement")]
    [Space(5)]
    [SerializeField]
    private float m_MovementSpeed;

    [SerializeField]
    private Transform m_TargetTransform;
    private Vector3 m_TargetPosition;
    private Vector3 m_StartPosition;

    [Space(10)]
    [Header("Scanning")]
    [Space(5)]
    [SerializeField]
    private Transform m_ViewPosition;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private SoldierFireState m_FireState;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Soldier = GetComponent<SoldierBehaviour>();

        m_StartPosition = transform.position;
        m_TargetPosition = m_TargetTransform.position;
    }

    public override void Enter()
    {
        Debug.Log("Entered patrolling state!");

        if (m_StartPosition != m_TargetPosition)
        {
            m_Soldier.NavMeshAgent.destination = m_TargetPosition;
        }

        m_Soldier.TriggerStayEvent += OnStateTriggerStay;
        m_Soldier.NavMeshAgent.Resume();
        m_Soldier.NavMeshAgent.speed = m_MovementSpeed;
        m_Soldier.Animator.SetTrigger("MovementTrigger");
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
    }

    private void HandleMovement()
    {
        if (m_StartPosition == m_TargetPosition)
            return;

        NavMeshAgent agent = m_Soldier.NavMeshAgent;
        Animator animator = m_Soldier.Animator;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            Vector3 temp = m_TargetPosition.Copy();
            m_TargetPosition = m_StartPosition;
            m_StartPosition = temp;

            agent.destination = m_TargetPosition;
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

                if (success && hitInfo.collider == other)
                {
                    //Change to the firing state
                    m_Soldier.SwitchState(m_FireState);
                    m_FireState.SetTarget(other);
                }
            }
        }
    }

    public override string ToString()
    {
        return "Patrolling";
    }
}