using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class HealthPanel : MonoBehaviour
    {
        [SerializeField]
        private Text m_HealthText = null;

        [SerializeField]
        private IDamageableObject m_DamageableObject = null;

        private void Awake()
        {
            m_DamageableObject.ChangeHealthEvent += OnUpdateHealth;
        }

        private void OnDestroy()
        {
            if (m_DamageableObject != null)
                m_DamageableObject.ChangeHealthEvent -= OnUpdateHealth;
        }

        public void OnUpdateHealth(int health)
        {
            m_HealthText.text = health.ToString();
        }
    }
}