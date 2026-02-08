using UnityEngine;

namespace Kweek
{
    public class CameraDuckMotion : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovementController m_Player = null;

        [SerializeField]
        private float m_Speed = 0.0f;

        [SerializeField]
        private Transform m_DuckedTransform = null;
        private Vector3 m_DuckedPosition = Vector3.zero;
        private Vector3 m_OriginalPosition = Vector3.zero;

        private Vector3 m_CurrentTargetPosition = Vector3.zero;

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
            if (isDucking) { m_CurrentTargetPosition = m_DuckedPosition; }
            else { m_CurrentTargetPosition = m_OriginalPosition; }
        }
    }
}