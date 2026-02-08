using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class PickupPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Visuals = null;

        [SerializeField]
        private Text m_PickupText = null;

        [SerializeField]
        private PickupHandler m_PickupHandler = null;

        private void Start()
        {
            m_PickupHandler.ChangeHoveredPickupEvent += OnChangePickup;
            m_Visuals.SetActive(false);
        }

        private void OnDestroy()
        {
            if (m_PickupHandler != null)
                m_PickupHandler.ChangeHoveredPickupEvent -= OnChangePickup;
        }

        public void OnChangePickup(IPickup pickup)
        {
            if (pickup == null)
            {
                m_Visuals.SetActive(false);
                return;
            }

            m_Visuals.SetActive(true);

            //TODO: Not actually bound to a key yet.
            m_PickupText.text = "Press [F] to pickup " + pickup.PickupName + ".";
        }
    }
}