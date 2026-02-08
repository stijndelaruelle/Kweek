using UnityEngine;

namespace Kweek
{
    public class DamageTestDummy : MonoBehaviour
    {
        [SerializeField]
        private IDamageableObject m_DamageableObject;

        private void Start()
        {
            if (m_DamageableObject != null)
            {
                m_DamageableObject.ChangeHealthEvent += OnChangeHealth;
                m_DamageableObject.DamageEvent += OnDamage;
                m_DamageableObject.DeathEvent += OnDeath;
            }
        }

        private void OnDestroy()
        {
            if (m_DamageableObject != null)
            {
                m_DamageableObject.ChangeHealthEvent -= OnChangeHealth;
                m_DamageableObject.DamageEvent -= OnDamage;
                m_DamageableObject.DeathEvent -= OnDeath;
            }
        }

        //Damage handling
        private void OnDamage(int removedHealth)
        {
            Debug.Log(gameObject.name + ": Hit for " + removedHealth + ".");
        }

        private void OnDeath()
        {
            //Debug.Log("DUMMY: Died!");
            m_DamageableObject.ChangeHealth(m_DamageableObject.MaxHealth);
        }

        private void OnChangeHealth(int health)
        {
            //Debug.Log("DUMMY: Health has been set to " + health + ".");
        }
    }
}