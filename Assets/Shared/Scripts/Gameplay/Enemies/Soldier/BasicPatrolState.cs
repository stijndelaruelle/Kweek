using System;
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
    private List<Transform> m_Path;
    private int m_CurrentIndex = 0;
    private bool m_IsStopped = false;

    [SerializeField]
    private bool m_LoopPath;

    [SerializeField]
    private bool m_BackAndForthPath;
    private int m_TraverseDirection = 1;

    [Space(10)]
    [Header("Scanning")]
    [Space(5)]
    [SerializeField]
    private Transform m_ViewPosition;

    [SerializeField]
    private float m_ViewRadius;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private LayerMask m_ScanLayerMask;

    [SerializeField]
    private IAbstractTargetState m_FireState;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Behaviour = GetComponent<EnemyWeaponPickupBehaviour>();
    }

    public override void Enter()
    {
        Debug.Log("Entered patrolling state!");

        if (m_Path != null && m_Path.Count > 0)
        {
            m_Behaviour.NavMeshAgent.destination = m_Path[m_CurrentIndex].transform.position;
            m_Behaviour.NavMeshAgent.speed = m_MovementSpeed;

            m_Behaviour.NavMeshAgent.isStopped = false;
            m_IsStopped = false;
        }
        else
        {
            m_Behaviour.NavMeshAgent.isStopped = true;
            m_IsStopped = true;
        }

        m_Behaviour.Animator.enabled = true;
        m_Behaviour.Animator.SetTrigger("MovementTrigger");
    }

    public override void Exit()
    {
        if (m_Behaviour == null)
            return;
    }

    public override void StateUpdate()
    {
        HandleMovement();
        HandleScanning();
    }

    private void HandleMovement()
    {
        if (m_IsStopped)
            return;

        NavMeshAgent agent = m_Behaviour.NavMeshAgent;

        if (agent == null )
            return;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            m_CurrentIndex += m_TraverseDirection;

            //Reached the end of our path
            if (m_CurrentIndex < 0 || m_CurrentIndex >= m_Path.Count)
            {
                if (m_LoopPath)              { m_CurrentIndex = Math.Abs(m_CurrentIndex - m_Path.Count); }
                else if (m_BackAndForthPath) { m_TraverseDirection = m_TraverseDirection * -1; m_CurrentIndex += m_TraverseDirection * 2; }
                else
                {
                    m_CurrentIndex -= m_TraverseDirection;
                    agent.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                    agent.isStopped = true;
                    m_IsStopped = true;
                    return;
                }
            }

            agent.destination = m_Path[m_CurrentIndex].transform.position;
        }
    }

    private void HandleScanning()
    {
        Collider[] colliders = Physics.OverlapSphere(m_ViewPosition.position, m_ViewRadius, m_ScanLayerMask);

        //For all targets in my radius
        for (int i = 0; i < colliders.Length; ++i)
        {
            Collider other = colliders[i];

            //Check if it's an enemy
            FactionType factionType = other.gameObject.GetComponent<FactionType>();
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
                    //Check if we can actually see him
                    Vector3 middleTop = other.bounds.center;
                    middleTop.y += other.bounds.extents.y * 0.5f;

                    Ray ray = new Ray(m_ViewPosition.position, (middleTop - m_ViewPosition.position));

                    RaycastHit hitInfo;
                    bool success = Physics.Raycast(ray, out hitInfo);

                    if (success && hitInfo.collider == other)
                    {
                        //Change to the firing state
                        if (m_FireState != null)
                        {
                            m_Behaviour.SwitchState(m_FireState);
                            m_FireState.SetTarget(damageableObject);
                        }
                    }
                }
            }
        } 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_ViewPosition.position, m_ViewRadius);
    }

    public override string ToString()
    {
        return "Patrolling";
    }
}