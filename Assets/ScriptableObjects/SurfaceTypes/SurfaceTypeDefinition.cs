using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTypeDefinition : ScriptableObject
{
    [Header("Piercing damage reduction")]
    [Space(5)]
    [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
    [SerializeField]
    private int m_PiercingDamageFalloffFlat;
    public int PiercingDamageFalloffFlat
    {
        get { return m_PiercingDamageFalloffFlat; }
    }

    [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
    [SerializeField]
    private int m_PiercingDamageFalloffPerUnit;
    public int PiercingDamageFalloffPerUnit
    {
        get { return m_PiercingDamageFalloffPerUnit; }
    }

    [Space(10)]
    [Header("Piercing range reduction")]
    [Space(5)]
    [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
    [SerializeField]
    private float m_PiercingRangeFalloffFlat;
    public float PiercingRangeFalloffFlat
    {
        get { return m_PiercingRangeFalloffFlat; }
    }

    [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
    [SerializeField]
    private float m_PiercingRangeFalloffPerUnit;
    public float PiercingRangeFalloffPerUnit
    {
        get { return m_PiercingRangeFalloffPerUnit; }
    }

    [Space(10)]
    [Header("Decals")]
    [Space(5)]
    [Tooltip("Decal left behind by a bullet that hits this surface type.")]
    [SerializeField]
    private GameObject m_DecalPrefab; //Change into a decal pool later
    public GameObject DecalPrefab
    {
        get { return m_DecalPrefab; }
    }

    [SerializeField]
    private List<Sprite> m_Decals;
    public List<Sprite> Decals
    {
        get { return m_Decals; }
    }

    //Hit particle
    //Hit sound
    //Walk sound
}
