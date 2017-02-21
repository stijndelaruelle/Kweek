using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Visuals;

    [SerializeField]
    private Text m_AmmoInClipText;

    [SerializeField]
    private Text m_AmmoInReserveText;

    [SerializeField]
    private WeaponArsenal m_WeaponArsenal;

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