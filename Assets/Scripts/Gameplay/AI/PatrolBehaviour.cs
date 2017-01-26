using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform m_TargetTransform;

    [SerializeField]
    private bool m_BackAndForth = true;

    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private Animator m_Animator;

    private NavMeshAgent m_NavMeshAgent;

    private Vector3 m_TargetPosition;
    private Vector3 m_StartPosition;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_StartPosition = transform.position.Copy();
        
        if (m_TargetTransform != null)
        {
            m_TargetPosition = m_TargetTransform.position.Copy();
            m_NavMeshAgent.destination = m_TargetTransform.position;
        }

        //Enable our animator
        m_Animator.enabled = true;
    }

    private void Update()
    {
        UpdateMovement();

        float velocity01 = (m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed) * m_Speed;

        m_Animator.SetFloat("Speed", velocity01);
    }

    private void UpdateMovement()
    {
        //Check if we reached our destination
        if (m_NavMeshAgent.remainingDistance <= 0.5f)
        {
            if (m_BackAndForth)
            {
                Vector3 temp = m_TargetPosition.Copy();
                m_TargetPosition = m_StartPosition;
                m_StartPosition = temp;

                m_NavMeshAgent.destination = m_TargetPosition;
            }
        }
    }

    public void Pause()
    {
        m_NavMeshAgent.Stop();
    }

    public void Resume()
    {
        m_NavMeshAgent.Resume();
    }
}