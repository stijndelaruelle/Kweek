using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEnemy : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Rigidbody m_MainRigidbody;
    public Rigidbody MainRigidbody
    {
        get { return m_MainRigidbody; }
    }

    private Rigidbody[] m_Rigidbodies;

    private void Awake()
    {
        //Disable gravity at the start
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].useGravity = false;
        }

        //Enable our animator
        m_Animator.enabled = true;
    }

    public void OnHit()
    {
        m_Animator.enabled = false;

        //for (int i = 0; i < m_Rigidbodies.Length; ++i)
        //{
        //    m_Rigidbodies[i].useGravity = true;
        //}
    }
}
