using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : BasicPickup
{
    [Header("Ammo")]
    [SerializeField]
    private AmmoTypeDefinition m_AmmoType;

    [SerializeField]
    private int m_Amount;

    public override void Pickup(Player player)
    {
        AmmoArsenal ammoArsenal = player.WeaponArsenal.AmmoArsenal;

        if (ammoArsenal != null)
        {
            int addedAmmo = m_Amount;

            int diff = m_AmmoType.MaxAmmo - ammoArsenal.GetAmmo(m_AmmoType);
            if (diff < addedAmmo)
                addedAmmo = diff;

            //If we wouldn't pick up any ammo, don't do anything.
            if (addedAmmo > 0)
            {
                ammoArsenal.ChangeAmmo(m_AmmoType, m_Amount);
                Destroy(gameObject);
            }

        }
    }
}