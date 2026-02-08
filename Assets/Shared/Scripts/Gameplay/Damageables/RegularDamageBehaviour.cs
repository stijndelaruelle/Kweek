using UnityEngine;

namespace Kweek
{
    public class RegularDamageBehaviour : IDamageableObject
    {
        [SerializeField]
        protected int m_MaxHealth = 0;
        public override int MaxHealth
        {
            get { return m_MaxHealth; }
        }

        protected int m_Health = 0;
        public override int Health
        {
            get { return m_Health; }
        }

        [SerializeField]
        private Transform m_Transform = null;

        private void Awake()
        {
            if (m_Transform == null)
                m_Transform = gameObject.GetComponent<Transform>();
        }

        private void Start()
        {
            ChangeHealth(m_MaxHealth);
        }

        public override int ChangeHealth(int health)
        {
            int prevHealth = m_Health;
            m_Health += health;

            int reserveHealth = 0;
            if (m_Health > m_MaxHealth)
            {
                reserveHealth = m_Health - m_MaxHealth;
                m_Health = m_MaxHealth;
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
            FireChangeHealthEvent(m_Health);

            return reserveHealth;
        }

        public override int Damage(int health)
        {
            if (IsDead())
                return health;

            int reserveHealth = ChangeHealth(-health);

            //Fire damage event
            FireDamageEvent(health);

            return reserveHealth;
        }

        public override int Heal(int health)
        {
            if (IsDead())
                return health;

            int reserveHealth = ChangeHealth(health);

            //Fire heal event
            FireHealEvent(health);

            return reserveHealth;
        }

        public override bool IsDead()
        {
            return (m_Health <= 0);
        }

        public override Vector3 GetPosition()
        {
            if (m_Transform == null)
                return Vector3.zero;
            
            return m_Transform.position;
        }

        public override IDamageableObject GetMainDamageableObject()
        {
            return this;
        }
    }
}