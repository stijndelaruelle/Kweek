using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Multiple weapons can use the same ammo type.
//In the case of RoZoSho you can for example have 2 AK's. But think Quake where both the grenade launcher & rocket launcher use "rocket" ammo.

public delegate void UpdateReserveAmmoDelegate(AmmoTypeDefinition ammoType, int i);

public class AmmoArsenal : MonoBehaviour
{
    [Serializable]
    private class AmmoReserve
    {
        [SerializeField]
        private AmmoTypeDefinition m_AmmoType;
        public AmmoTypeDefinition AmmoType
        {
            get { return m_AmmoType; }
        }

        [SerializeField]
        private int m_Amount;
        public int Amount
        {
            get { return m_Amount; }
            set { m_Amount = value; }
        }
    }

    [SerializeField]
    private List<AmmoReserve> m_Ammo;

    //Event
    private UpdateReserveAmmoDelegate m_UpdateReserveAmmoEvent;
    public UpdateReserveAmmoDelegate UpdateReserveAmmoEvent
    {
        get { return m_UpdateReserveAmmoEvent; }
        set { m_UpdateReserveAmmoEvent = value; }
    }

    public int GetAmmo(AmmoTypeDefinition ammoType)
    {
        int index = GetIndexFromDefintion(ammoType);
        if (index < 0)
            return -1;

        return m_Ammo[index].Amount;
    }

    public void ChangeAmmo(AmmoTypeDefinition ammoType, int amount)
    {
        int index = GetIndexFromDefintion(ammoType);
        if (index < 0)
            return;

        m_Ammo[index].Amount += amount;

        if (m_Ammo[index].Amount < 0)
            m_Ammo[index].Amount = 0;

        if (m_Ammo[index].Amount > ammoType.MaxAmmo)
            m_Ammo[index].Amount = ammoType.MaxAmmo;

        FireUpdateReserveAmmoEvent(ammoType, m_Ammo[index].Amount);
    }

    private int GetIndexFromDefintion(AmmoTypeDefinition ammoType)
    {
        //Change into linq expression at some point. Very lame linear search for now
        for (int i = 0; i < m_Ammo.Count; ++i)
        {
            if (m_Ammo[i].AmmoType == ammoType)
                return i;
        }

        return -1;
    }

    private void FireUpdateReserveAmmoEvent(AmmoTypeDefinition ammoType, int amount)
    {
        if (m_UpdateReserveAmmoEvent != null)
            m_UpdateReserveAmmoEvent(ammoType, amount);
    }
}
