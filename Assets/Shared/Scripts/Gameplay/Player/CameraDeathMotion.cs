using UnityEngine;

namespace Kweek
{
    public class CameraDeathMotion : MonoBehaviour
    {
        [SerializeField]
        private Player m_Player = null;

        [SerializeField]
        private Collider m_PlayerCollider = null;

        [Space(10)]
        [Header("Position")]
        [Space(5)]
        [SerializeField]
        private Transform m_TargetTransform = null;
        private Vector3 m_DefaultPosition = Vector3.zero;

        [SerializeField]
        private float m_MovementSpeed = 0.0f;

        [Space(10)]
        [Header("Rotation")]
        [Space(5)]
        [SerializeField]
        private Transform m_FollowObject = null;
        private bool m_IsFollowing = false;

        [SerializeField]
        private float m_RotationSpeed = 0.0f;

        //"Cheap" way to make sure we don't go trough walls
        [Space(10)]
        [Header("Physics")]
        [Space(5)]

        [SerializeField]
        private Rigidbody m_RigidBody = null;

        [SerializeField]
        private Collider m_Collider = null;

        private void Awake()
        {
            m_DefaultPosition = transform.localPosition.Copy();
        }

        private void Start()
        {
            if (m_Player != null)
            {
                m_Player.DeathEvent += OnPlayerDeath;
                m_Player.RespawnEvent += OnPlayerRespawn;
            }

            //Ignore collision between the player and the camera
            //Physics.IgnoreCollision(m_PlayerCollider, m_Collider, true);

            Reset();
        }

        private void OnDestroy()
        {
            if (m_Player != null)
            {
                m_Player.DeathEvent -= OnPlayerDeath;
                m_Player.RespawnEvent -= OnPlayerRespawn;
            }
        }

        private void Update()
        {
            if (m_IsFollowing)
            {
                Quaternion targetRotation = Quaternion.LookRotation(m_FollowObject.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
            }
        }

        private void Reset()
        {
            m_IsFollowing = false;

            transform.localPosition = m_DefaultPosition;

            m_RigidBody.isKinematic = true;
            m_Collider.enabled = false;
        }

        private void OnPlayerDeath()
        {
            if (m_IsFollowing == true)
                return;

            m_IsFollowing = true;

            m_RigidBody.isKinematic = false;
            m_Collider.enabled = true;

            Vector3 diff = (m_TargetTransform.position - transform.position).normalized;
            m_RigidBody.AddForce(diff * m_MovementSpeed, ForceMode.Impulse);
        }

        private void OnPlayerRespawn()
        {
            Reset();
        }
    }
}