using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : IAIBehaviour
{
    [SerializeField]
    protected IAbstractState m_InitialState;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    protected Animator m_Animator;
    public Animator Animator
    {
        get { return m_Animator; }
    }

    [SerializeField]
    protected NavMeshAgent m_NavMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get { return m_NavMeshAgent; }
    }

    [SerializeField]
    private FactionTypeDefinition m_FactionTypeDefinition;
    public FactionTypeDefinition FactionType
    {
        get { return m_FactionTypeDefinition; }
    }

    [SerializeField]
    protected UnityMethodsForwarder m_Forwarder;

    private List<Collider> m_Colliders;
    private List<int> m_ColliderLayers;

    protected IState m_CurrentState;
    protected float m_LastSpeed; //Sometines the velocity can spike, if so reset to this value

    //Events
    //public event TriggerDelegate TriggerEnterEvent;
    //public event TriggerDelegate TriggerStayEvent; //Way too taxing!
    //public event TriggerDelegate TriggerExitEvent;
    public event AnimatorIKDelegate AnimatorIKEvent;

    protected virtual void Start()
    {
        m_Forwarder.AnimatorIKEvent += OnAnimatorIK;
        SwitchState(m_InitialState);
    }

    protected virtual void OnDestroy()
    {
        if (m_Forwarder != null)
            m_Forwarder.AnimatorIKEvent -= OnAnimatorIK;
    }

    //IAIBehaviour
    public override void Setup(List<Collider> ownerColliders)
    {
        m_Colliders = ownerColliders;

        m_ColliderLayers = new List<int>();
        foreach (Collider collider in m_Colliders)
        {
            m_ColliderLayers.Add(collider.gameObject.layer);
        }
    }

    public override void OnDeath()
    {
        SwitchState(null);

        //Disable our navmesh
        m_NavMeshAgent.enabled = false;
    }

    protected virtual void Update()
    {
        if (m_CurrentState != null)
            m_CurrentState.StateUpdate();

        HandleMovementAnimation();
    }

    private void HandleMovementAnimation()
    {
        //Walking animation
        float currentSpeed = m_NavMeshAgent.velocity.magnitude;

        //Every once in a while the velocity will spike (fix this)
        if (currentSpeed > m_NavMeshAgent.speed) { currentSpeed = m_LastSpeed; }
        m_LastSpeed = currentSpeed;

        m_Animator.SetFloat("VelocityX", 0.0f);
        m_Animator.SetFloat("VelocityZ", currentSpeed);
    }

    public IState SwitchState(IState newState)
    {
        if (m_CurrentState == newState)
        {
            //Debug.LogWarning("State is trying to switch to itself, something is wrong.");
            return m_CurrentState;
        }

        if (m_CurrentState != null)
            m_CurrentState.Exit();

        m_CurrentState = newState;

        if (m_CurrentState != null)
            m_CurrentState.Enter();

        return m_CurrentState;
    }

    //protected virtual void OnTriggerEnter(Collider other)
    //{
    //    if (TriggerEnterEvent != null)
    //        TriggerEnterEvent(other);
    //}

    //Wat too taxing!
    //protected virtual void OnTriggerStay(Collider other)
    //{
    //    if (TriggerStayEvent != null)
    //        TriggerStayEvent(other);
    //}

    //protected virtual void OnTriggerExit(Collider other)
    //{
    //    if (TriggerExitEvent != null)
    //        TriggerExitEvent(other);
    //}

    public void MakeCollidersIgnoreRaycasts(bool value)
    {
        for (int i = 0; i < m_Colliders.Count; ++i)
        {
            int layerID = 2;
            if (value == false) { layerID = m_ColliderLayers[i]; }

            m_Colliders[i].gameObject.layer = layerID;
        }
    }

    protected virtual void OnAnimatorIK(int layerIndex)
    {
        if (AnimatorIKEvent != null)
            AnimatorIKEvent(layerIndex);
    }
}
