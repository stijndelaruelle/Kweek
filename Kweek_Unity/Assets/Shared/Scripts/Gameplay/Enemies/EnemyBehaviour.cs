using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Kweek
{
    public class EnemyBehaviour : IAIBehaviour
    {
        [SerializeField]
        protected IAbstractState m_InitialState = null;

        [Space(10)]
        [Header("Required references")]
        [Space(5)]
        [SerializeField]
        protected Animator m_Animator = null;
        public Animator Animator
        {
            get { return m_Animator; }
        }

        [SerializeField]
        protected NavMeshAgent m_NavMeshAgent = null;
        public NavMeshAgent NavMeshAgent
        {
            get { return m_NavMeshAgent; }
        }

        [SerializeField]
        private FactionTypeDefinition m_FactionTypeDefinition = null;
        public FactionTypeDefinition FactionType
        {
            get { return m_FactionTypeDefinition; }
        }

        [SerializeField]
        protected UnityMethodsForwarder m_Forwarder = null;

        private List<Collider> m_Colliders = null;
        private List<int> m_ColliderLayers = null;

        protected IState m_CurrentState = null;
        protected float m_LastSpeed = 0.0f; //Sometines the velocity can spike, if so reset to this value

        //Events
        //public event TriggerDelegate TriggerEnterEvent;
        //public event TriggerDelegate TriggerStayEvent; //Way too taxing!
        //public event TriggerDelegate TriggerExitEvent;
        public event AnimatorIKDelegate AnimatorIKEvent = null;

        protected virtual void Start()
        {
            if (m_Forwarder != null)
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
                if (collider == null)
                    continue;

                m_ColliderLayers.Add(collider.gameObject.layer);
            }
        }

        public override void OnDeath()
        {
            SwitchState(null);

            //Disable our navmesh
            if (m_NavMeshAgent != null)
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
            if (currentSpeed > m_NavMeshAgent.speed)
                currentSpeed = m_LastSpeed;

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
            if (m_Colliders == null || m_Colliders.Count == 0)
                return;

            for (int i = 0; i < m_Colliders.Count; ++i)
            {
                Collider collider = m_Colliders[i];
                if (collider == null)
                    continue;

                int layerID = 2;

                if (value == false && m_ColliderLayers != null && m_ColliderLayers.Count > i)
                    layerID = m_ColliderLayers[i];

                collider.gameObject.layer = layerID;
            }
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (AnimatorIKEvent != null)
                AnimatorIKEvent(layerIndex);
        }

        //TODO: Create Animator pass through methods
    }
}