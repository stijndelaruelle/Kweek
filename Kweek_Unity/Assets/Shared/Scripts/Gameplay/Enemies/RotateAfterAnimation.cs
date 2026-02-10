using UnityEngine;

namespace Kweek
{
    public class RotateAfterAnimation : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator = null;

        [SerializeField]
        private Vector3 m_EulerAngles = Vector3.zero;

        //Cache
        private Transform m_Transform = null;

        private void Awake()
        {
            m_Transform = gameObject.GetComponent<Transform>();
        }

        private void LateUpdate()
        {
            if (m_Transform == null || m_Animator.enabled == false)
                return;

            //Rotate ourselves to the target direction, our model is a bit offset so we can't let the navmeshagent handle it.
            m_Transform.rotation *= Quaternion.Euler(m_EulerAngles);
        }
    }
}