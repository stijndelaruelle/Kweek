using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBehaviour))]
public class BasicSearchChaseState : IAbstractTargetState
{
    private EnemyBehaviour m_Behaviour;

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
    private float m_ViewRadius;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private LayerMask m_ScanLayerMask;

    [SerializeField]
    private BasicPatrolState m_PatrolState;

    [SerializeField]
    private IAbstractTargetState m_FireState;


    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Behaviour = GetComponent<EnemyBehaviour>();
    }

    public override void Enter()
    {
        Debug.Log("Entered Chase state!");

        m_Behaviour.NavMeshAgent.isStopped = false;
        m_Behaviour.NavMeshAgent.speed = m_MovementSpeed;
        m_Behaviour.NavMeshAgent.destination = m_TargetPosition;

        m_Behaviour.Animator.enabled = true;
        m_Behaviour.Animator.SetTrigger("MovementTrigger");

        m_ChaseTimer = 0.0f;
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

        m_ChaseTimer += Time.deltaTime;
    }

    private void HandleMovement()
    {
        NavMeshAgent agent = m_Behaviour.NavMeshAgent;
        Animator animator = m_Behaviour.Animator;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            //If we still didn't switch to the fire state at this point, we lost the player. Go back to patrolling
            m_Behaviour.SwitchState(m_PatrolState);
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
                    Ray ray = new Ray(m_ViewPosition.position, (damageableObject.GetPosition() - m_ViewPosition.position));

                    RaycastHit hitInfo;
                    bool success = Physics.Raycast(ray, out hitInfo);

                    if (success && hitInfo.collider == other && m_ChaseTimer >= m_MinChaseTime)
                    {
                        //Change to the firing state
                        m_Behaviour.SwitchState(m_FireState);
                        m_FireState.SetTarget(damageableObject);
                    }
                }
            }
        }
    }

    public override void SetTarget(IDamageableObject target)
    {
        m_TargetPosition = target.transform.position;
        m_Behaviour.NavMeshAgent.destination = m_TargetPosition;
    }

    public override string ToString()
    {
        return "Chasing";
    }
}