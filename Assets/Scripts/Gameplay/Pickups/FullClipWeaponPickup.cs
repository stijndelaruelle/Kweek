using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullClipWeaponPickup : MonoBehaviour, IPickup
{
    [SerializeField]
    private string m_PickupName;
    public string PickupName
    {
        get { return m_PickupName; }
    }

    [SerializeField]
    private Weapon m_FirstPersonWeapon;

    [Header("Ammo")]
    [SerializeField]
    private int m_AmmoInClip;

    [SerializeField]
    private int m_AmmoInReserve;

    public void Pickup(Player player)
    {
        WeaponArsenal weaponArsenal = player.WeaponArsenal;
        Weapon newWeapon = weaponArsenal.AddWeapon(m_FirstPersonWeapon);

        FullClipReloadBehaviour reloadBehaviour = newWeapon.GetComponent<FullClipReloadBehaviour>();

        if (reloadBehaviour != null)
        {
            reloadBehaviour.SetAmmo(m_AmmoInClip, m_AmmoInReserve);
        }

        Destroy(gameObject);
    }
}
