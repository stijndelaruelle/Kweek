﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBehaviour))]
public class BasicWanderState : IAbstractState
{
    private EnemyBehaviour m_Zombie;

    [Header("Movement")]
    [Space(5)]
    [SerializeField]
    private float m_MovementSpeed;

    [SerializeField]
    private float m_MinWanderRadius;

    [SerializeField]
    private float m_MaxWanderRadius;

    [SerializeField]
    private float m_MinWaitTime;

    [SerializeField]
    private float m_MaxWaitTime;
    private Coroutine m_WanderInterbellumRoutine;

    [Space(10)]
    [Header("Scanning")]
    [Space(5)]
    [SerializeField]
    private IAbstractTargetState m_TargetState;

    [SerializeField]
    private Transform m_ViewPosition;

    [SerializeField]
    private float m_ViewRadius;

    [SerializeField]
    private float m_ViewAngle;

    [SerializeField]
    private LayerMask m_ScanLayerMask;

    private void Awake()
    {
        //Assigning this manually clutters the inspector a LOT!
        //If we, at some point, want to detach state objects from their behaviour, revert this.
        m_Zombie = GetComponent<EnemyBehaviour>();
    }

    public override void Enter()
    {
        //Debug.Log("Entered wandering state!");

        //m_Zombie.TriggerStayEvent += OnStateTriggerStay;

        m_Zombie.NavMeshAgent.destination = transform.position;
        m_Zombie.NavMeshAgent.speed = m_MovementSpeed;

        m_Zombie.NavMeshAgent.isStopped = true;

        m_Zombie.Animator.enabled = true;
        m_Zombie.Animator.SetTrigger("MovementTrigger");
    }

    public override void Exit()
    {
        //if (m_Zombie != null)
        //    m_Zombie.TriggerStayEvent -= OnStateTriggerStay;

        if (m_WanderInterbellumRoutine != null)
            StopCoroutine(m_WanderInterbellumRoutine);
    }

    public override void StateUpdate()
    {
        HandleMovement();
        HandleScanning();
    }

    private void HandleMovement()
    {
        NavMeshAgent agent = m_Zombie.NavMeshAgent;

        if (agent == null)
            return;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            if (m_WanderInterbellumRoutine == null)
            {
                m_WanderInterbellumRoutine = StartCoroutine(WanderInterbellumRoutine());
            }
        }
    }

    private IEnumerator WanderInterbellumRoutine()
    {
        m_Zombie.NavMeshAgent.isStopped = true;

        float waitTime = Random.Range(m_MinWaitTime, m_MaxWaitTime);
        yield return new WaitForSeconds(waitTime);

        m_Zombie.NavMeshAgent.destination = CalculateTargetPosition();
        m_Zombie.NavMeshAgent.isStopped = false;

        m_WanderInterbellumRoutine = null;
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

            if (m_Zombie.FactionType.IsEnemy(factionType.Faction))
            {
                IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
                if (damageableObject == null)
                    return;

                damageableObject = damageableObject.GetMainDamageableObject();

                if (damageableObject.IsDead())
                    return;

                //If so check if he's within the specified angle
                Vector3 diffPos = other.transform.position - m_Zombie.transform.position;
                float dot = Vector3.Dot(m_Zombie.transform.forward, diffPos.normalized);
                float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

                if (degAngle <= m_ViewAngle)
                {
                    //Check if there is line of sight
                    Ray ray = new Ray(m_ViewPosition.position, (damageableObject.GetPosition() - m_ViewPosition.position));

                    RaycastHit hitInfo;
                    bool success = Physics.Raycast(ray, out hitInfo);

                    if (success && hitInfo.collider == other)
                    {
                        //Change to the chasing state
                        m_TargetState.SetTarget(damageableObject);
                        m_Zombie.SwitchState(m_TargetState);
                    }
                }
            }
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = transform.position.Copy();

        targetPosition.x += Random.Range(m_MinWanderRadius, m_MaxWanderRadius) * Mathf.Sign(Random.Range(-100, 100f));
        targetPosition.z += Random.Range(m_MinWanderRadius, m_MaxWanderRadius) * Mathf.Sign(Random.Range(-100, 100f));

        return targetPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_ViewPosition.position, m_ViewRadius);
    }

    public override string ToString()
    {
        return "Wandering";
    }
}