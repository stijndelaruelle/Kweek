using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollingEnemy : MonoBehaviour
{
    [SerializeField]
    private Transform m_TargetTransform;

    [SerializeField]
    private bool m_BackAndForth = true;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Rigidbody m_MainRigidbody;
    public Rigidbody MainRigidbody
    {
        get { return m_MainRigidbody; }
    }

    private Rigidbody[] m_Rigidbodies;


    private NavMeshAgent m_NavMeshAgent;

    private Vector3 m_TargetPosition;
    private Vector3 m_StartPosition;

    private bool m_InHitStun = false;

    private void Awake()
    {
        m_StartPosition = transform.position.Copy();
        m_TargetPosition = m_TargetTransform.position.Copy();

        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.destination = m_TargetTransform.position;


        //Disable gravity at the start
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].useGravity = false;
        }

        //Enable our animator
        m_Animator.enabled = true;
    }

    private void Update()
    {
        //Update passengers first, otherwise there is a snap when changing directions
        UpdateMovement();

        float velocity01 = (m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed) * 0.5f; //half speed

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

    public void OnHit()
    {
        if (m_InHitStun)
            return;

        m_NavMeshAgent.Stop();
        m_Animator.SetTrigger("WoundTrigger");
        m_InHitStun = true;

        //m_Animator.enabled = false;
        //for (int i = 0; i < m_Rigidbodies.Length; ++i)
        //{
        //    m_Rigidbodies[i].useGravity = true;
        //}
    }

    public void OnEndHitStun()
    {
        m_NavMeshAgent.Resume();
        m_InHitStun = false;
    }
}