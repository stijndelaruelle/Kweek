using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDeathMotion : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private Collider m_PlayerCollider;

    [Space(10)]
    [Header("Position")]
    [Space(5)]
    [SerializeField]
    private Transform m_TargetPosition;
    private Vector3 m_DefaultPosition;

    [SerializeField]
    private float m_MovementSpeed;

    [Space(10)]
    [Header("Rotation")]
    [Space(5)]
    [SerializeField]
    private Transform m_FollowObject;
    private bool m_IsFollowing = false;

    [SerializeField]
    private float m_RotationSpeed;

    //"Cheap" way to make sure we don't go trough walls
    [Space(10)]
    [Header("Physics")]
    [Space(5)]

    [SerializeField]
    private Rigidbody m_RigidBody;

    [SerializeField]
    private Collider m_Collider;

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

        Vector3 diff = (m_TargetPosition.position - transform.position).normalized;
        m_RigidBody.AddForce(diff * m_MovementSpeed, ForceMode.Impulse);
    }

    private void OnPlayerRespawn()
    {
        Reset();
    }
}
