using UnityEngine;

namespace Kweek
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private IDamageableObject m_DamageableObject = null;
        public IDamageableObject DamageableObject
        {
            get { return m_DamageableObject; }
        }

        [SerializeField]
        private WeaponArsenal m_WeaponArsenal = null;
        public WeaponArsenal WeaponArsenal
        {
            get { return m_WeaponArsenal; }
        }

        //Events
        public event DeathDelegate DeathEvent = null;
        public event DeathDelegate RespawnEvent = null;

        private void Start()
        {
            m_DamageableObject.DeathEvent += OnDeath;
        }

        private void OnDestroy()
        {
            if (m_DamageableObject != null)
                m_DamageableObject.DeathEvent -= OnDeath;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Respawn();
            }
        }

        private void OnDeath()
        {
            Debug.Log("THE PLAYER DIED!");

            if (DeathEvent != null)
                DeathEvent();
        }

        private void Respawn()
        {
            //For testing purposes, normally you would never respawn but load a gamestate.

            //Max health
            m_DamageableObject.ChangeHealth(m_DamageableObject.MaxHealth);

            if (RespawnEvent != null)
                RespawnEvent();
        }
    }
}