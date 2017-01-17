using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For now a simple enum, will refactor on expansion of the feature.
public enum AmmoType
{
    PistolAmmo,
    AK47Ammo,
    M4Ammo,
    ShotgunAmmo
}

//Multiple weapons can use the same ammo type.
//In the case of RoZoSho you can for example have 2 AK's. But think Quake where both the grenade launcher & rocket launcher use "rocket" ammo.
public class AmmoArsenal : MonoBehaviour
{
    [SerializeField]
    private List<AmmoType> m_AmmoTypes; //TODO MAKE THIS SCRIPTABLE OBJECTS, MAX AMMO PER TYPE INSTELLEN!
    private List<int> m_AmmoReserve;

    private void Awake()
    {
        int ammoTypes = Enum.GetNames(typeof(AmmoType)).Length;
        m_AmmoReserve = new List<int>(ammoTypes);
        for (int i = 0; i < ammoTypes; ++i) { m_AmmoReserve.Add(0); }
    }

    public int GetAmmo(AmmoType ammoType)
    {
        return m_AmmoReserve[(int)ammoType];
    }

    public void ChangeAmmo(AmmoType ammoType, int amount)
    {
        m_AmmoReserve[(int)ammoType] += amount;
    }
}
