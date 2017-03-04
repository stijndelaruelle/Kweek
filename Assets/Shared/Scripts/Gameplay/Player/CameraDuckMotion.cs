using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDuckMotion : MonoBehaviour
{
    [SerializeField]
    private PlayerMovementController m_Player;

    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private Transform m_DuckedTransform;
    private Vector3 m_DuckedPosition;
    private Vector3 m_OriginalPosition;

    private Vector3 m_CurrentTargetPosition;

    private void Awake()
    {
        m_OriginalPosition = transform.localPosition.Copy();
        m_DuckedPosition = m_DuckedTransform.localPosition.Copy();

        m_CurrentTargetPosition = m_OriginalPosition;
    }

    private void Start()
    {
        m_Player.DuckEvent += OnPlayerDuck;
        m_Player.UpdateCameraEvent += OnUpdateCameraEvent;
    }

    private void OnDestroy()
    {
        if (m_Player != null)
        {
            m_Player.DuckEvent -= OnPlayerDuck;
            m_Player.UpdateCameraEvent -= OnUpdateCameraEvent;
        }
    }

    private void OnUpdateCameraEvent()
    {
        if (transform.localPosition != m_CurrentTargetPosition)
        {
            Vector3 dir = (m_CurrentTargetPosition - transform.localPosition).normalized;

            transform.localPosition += dir * m_Speed * Time.deltaTime;

            //If we switched directions, we arrived. (lame way)
            Vector3 dirAfter = (m_CurrentTargetPosition - transform.localPosition).normalized;
            if (dir != dirAfter)
            {
                transform.localPosition = m_CurrentTargetPosition;
            }
        }
    }

    private void OnPlayerDuck(bool isDucking)
    {
        if (isDucking) { m_CurrentTargetPosition = m_DuckedPosition;   }
        else           { m_CurrentTargetPosition = m_OriginalPosition; }
    }
}
