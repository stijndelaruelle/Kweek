using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTypeDefinition : ScriptableObject
{
    [Tooltip("Maximum amount of ammo we can have of this ammo type")]
    [SerializeField]
    private int m_MaxAmmo;
    public int MaxAmmo
    {
        get { return m_MaxAmmo; }
    }
}
