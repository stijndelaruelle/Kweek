using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : BasicPickup
{
    [Header("Ammo")]
    [SerializeField]
    private int m_Ammo;

    public override void Pickup(Player player)
    {
        IDamageableObject damageableObject = player.DamageableObject;

        if (damageableObject != null)
        {

        }
    }
}