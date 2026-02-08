using UnityEngine;

namespace Kweek
{
    //This is the part that has the collider
    //It sends the damage to the main damageableobject
    public class DamageablePart : IDamageableObject
    {
        [SerializeField]
        private IDamageableObject m_MainObject = null;
        public IDamageableObject MainObject
        {
            get { return m_MainObject; }
        }

        public override int MaxHealth
        {
            get
            {
                if (m_MainObject == null)
                    return 0;

                return m_MainObject.MaxHealth;
            }
        }
        public override int Health
        {
            get
            {
                if (m_MainObject == null)
                    return 0;

                return m_MainObject.Health;
            }
        }

        [SerializeField]
        private float m_DamageMultiplier = 1.0f;

        //Cache
        private Transform m_Transform = null;

        private void Awake()
        {
            m_Transform = gameObject.GetComponent<Transform>();
        }

        public override int ChangeHealth(int health)
        {
            if (m_MainObject == null)
                return 0;

            int reserveHealth = m_MainObject.ChangeHealth(health);
            FireChangeHealthEvent(health);

            return reserveHealth;
        }

        public override int Damage(int health)
        {
            if (m_MainObject == null)
                return 0;

            int actualDamage = Mathf.CeilToInt(health * m_DamageMultiplier);

            int reserveDamage = m_MainObject.Damage(actualDamage);
            FireDamageEvent(actualDamage);

            return reserveDamage;
        }

        public override int Heal(int health)
        {
            if (m_MainObject == null)
                return 0;

            int actualHealing = Mathf.CeilToInt(health * m_DamageMultiplier);

            int reserveHealth = m_MainObject.Heal(actualHealing);
            FireHealEvent(actualHealing);

            return reserveHealth;
        }

        public override bool IsDead()
        {
            if (m_MainObject == null)
                return true;

            return m_MainObject.IsDead();
        }

        public override Vector3 GetPosition()
        {
            if (m_Transform == null)
                return Vector3.zero;

            return m_Transform.position;
        }

        public override IDamageableObject GetMainDamageableObject()
        {
            return m_MainObject;
        }
    }
}
