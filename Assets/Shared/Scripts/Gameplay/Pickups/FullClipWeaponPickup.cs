﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullClipWeaponPickup : WeaponPickup
{
    public override void Pickup(Player player)
    {
        base.Pickup(player);

        FullClipReloadBehaviour reloadBehaviour = m_FirstPersonWeaponInstance.GetComponent<FullClipReloadBehaviour>();
        if (reloadBehaviour != null)
        {
            reloadBehaviour.SetAmmo(m_Ammo);
        }
    }
}
