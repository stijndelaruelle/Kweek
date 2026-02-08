using UnityEngine;

namespace Kweek
{
    public class OverhealDamageBehaviour : RegularDamageBehaviour
    {
        [Header("Overheal Settings")]
        [Space(5)]
        [SerializeField]
        private int m_MaxOverhealHealth = 0;

        [SerializeField]
        [Tooltip("Time it takes for 1 hitpoint to dissapear.")]
        private float m_OverhealDecayRate = 0.0f;
        private float m_OverhealDecayTimer = 0.0f;

        private void Update()
        {
            //Overheal decay
            if (m_Health > m_MaxHealth)
            {
                m_OverhealDecayTimer += Time.deltaTime;

                if (m_OverhealDecayTimer > m_OverhealDecayRate)
                {
                    m_OverhealDecayTimer -= m_OverhealDecayRate;
                    ChangeHealth(-1); //Decay per 1 (TODO: make it an option?)
                }

                if (m_Health <= m_MaxHealth)
                {
                    m_OverhealDecayTimer = 0.0f;
                }
            }
        }

        public override int ChangeHealth(int health)
        {
            int prevHealth = m_Health;
            m_Health += health;

            int reserveHealth = 0;

            if (m_Health > m_MaxOverhealHealth)
            {
                reserveHealth = m_Health - m_MaxOverhealHealth;
                m_Health = m_MaxOverhealHealth;
            }

            if (m_Health <= 0)
            {
                reserveHealth = Mathf.Abs(m_Health);
                m_Health = 0;

                //Fire death event
                if (prevHealth != 0)
                    FireDeathEvent();
            }

            //Fire healthchange event
            if (m_Health != prevHealth)
                FireChangeHealthEvent(m_Health);

            return reserveHealth;
        }
    }
}