using UnityEngine;

namespace Kweek
{
    public class PickupHandler : MonoBehaviour
    {
        [SerializeField]
        private Player m_Player = null;

        [SerializeField]
        private float m_Range = 0.0f;
        private bool m_IsEnabled = true;
        private IPickup m_CurrentHoveredPickup = null;

        //Event
        public event ChangePickupDelegate ChangeHoveredPickupEvent = null;

        private void Start()
        {
            if (m_Player != null)
            {
                m_Player.DeathEvent += OnPlayerDeath;
                m_Player.RespawnEvent += OnPlayerRespawn;
            }
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
            if (!m_IsEnabled)
                return;

            UpdateHoveredPickup();
            HandleInput();
        }

        private void UpdateHoveredPickup()
        {
            //Fire a single ray from the middle of the screen (get only the first target)
            Ray centerScreenRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            RaycastHit hitInfo = default(RaycastHit);
            bool success = Physics.Raycast(centerScreenRay, out hitInfo, m_Range);

            Debug.DrawRay(centerScreenRay.origin, centerScreenRay.direction * m_Range, Color.red);

            if (success == false)
            {
                SetHoveredPickup(null);
                return;
            }

            //Did we hit a pickup?
            GameObject hitGameObject = hitInfo.collider.gameObject;
            IPickup pickup = hitGameObject.GetComponent<IPickup>();

            SetHoveredPickup(pickup); //Can still be null at this point
        }

        private void HandleInput()
        {
            if (m_CurrentHoveredPickup == null)
                return;

            if (Input.GetButtonDown("Use")) //Input.GetKeyDown(KeyCode.E)
                m_CurrentHoveredPickup.Pickup(m_Player);
        }

        private void SetHoveredPickup(IPickup pickup)
        {
            if (pickup == m_CurrentHoveredPickup)
                return;

            m_CurrentHoveredPickup = pickup;

            if (ChangeHoveredPickupEvent != null)
                ChangeHoveredPickupEvent(pickup);
        }

        //Callbacks
        private void OnPlayerDeath()
        {
            m_IsEnabled = false;
            SetHoveredPickup(null);
        }

        private void OnPlayerRespawn()
        {
            m_IsEnabled = true;
        }
    }
}