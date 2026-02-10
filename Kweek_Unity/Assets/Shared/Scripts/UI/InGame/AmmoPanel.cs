using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class AmmoPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Visuals = null;

        [SerializeField]
        private Text m_AmmoInClipText = null;

        [SerializeField]
        private Text m_AmmoInReserveText = null;

        [SerializeField]
        private WeaponArsenal m_WeaponArsenal = null;

        private void Start()
        {
            m_WeaponArsenal.UpdateAmmoEvent += OnUpdateAmmo;
        }

        private void OnDestroy()
        {
            if (m_WeaponArsenal != null)
                m_WeaponArsenal.UpdateAmmoEvent -= OnUpdateAmmo;
        }

        public void OnUpdateAmmo(int ammoInClip, int ammoInReserve)
        {
            m_Visuals.SetActive(true);
            m_AmmoInClipText.text = ammoInClip.ToString();
            m_AmmoInReserveText.text = ammoInReserve.ToString();
        }
    }
}