using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBehaviour))]
public class BasicPatrolState : IAbstractState
{
    private EnemyBehaviour m_Behaviour;

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
    private IAbstractTargetState m_FireState;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Behaviour = GetComponent<EnemyWeaponPickupBehaviour>();

        m_StartPosition = transform.position;

        if (m_TargetTransform != null)
            m_TargetPosition = m_TargetTransform.position;
    }

    public override void Enter()
    {
        //Debug.Log("Entered patrolling state!");

        m_Behaviour.TriggerStayEvent += OnStateTriggerStay;

        if (m_TargetTransform != null)
        {
            m_Behaviour.NavMeshAgent.destination = m_TargetPosition;
            m_Behaviour.NavMeshAgent.speed = m_MovementSpeed;

            m_Behaviour.NavMeshAgent.Resume();
        }
        else
        {
            m_Behaviour.NavMeshAgent.Stop();
        }

        m_Behaviour.Animator.enabled = true;
        m_Behaviour.Animator.SetTrigger("MovementTrigger");
    }

    public override void Exit()
    {
        if (m_Behaviour == null)
            return;

        m_Behaviour.TriggerStayEvent -= OnStateTriggerStay;
    }

    public override void StateUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (m_StartPosition == m_TargetPosition)
            return;

        NavMeshAgent agent = m_Behaviour.NavMeshAgent;

        if (agent == null)
            return;

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
        //Check if it's an enemy
        FactionType factionType = other.GetComponent<FactionType>();
        if (factionType == null)
            return;

        if (m_Behaviour.FactionType.IsEnemy(factionType.Faction))
        {
            IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
            if (damageableObject == null)
                return;

            damageableObject = damageableObject.GetMainDamageableObject();

            if (damageableObject.IsDead())
                return;

            //If so check if he's within the specified angle
            Vector3 diffPos = other.transform.position - m_Behaviour.transform.position;
            float dot = Vector3.Dot(m_Behaviour.transform.forward, diffPos.normalized);
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
                    m_Behaviour.SwitchState(m_FireState);
                    m_FireState.SetTarget(damageableObject);
                }
            }
        }
    }

    public override string ToString()
    {
        return "Patrolling";
    }
}