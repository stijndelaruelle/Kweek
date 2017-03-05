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
    [Header("Weapon Impact effects")]
    [Space(5)]
    [SerializeField]
    private PoolableObject m_WeaponImpactEffectPrefab;
    public PoolableObject WeaponImpactEffectPrefab
    {
        get { return m_WeaponImpactEffectPrefab; }
    }

    [Tooltip("Decal left behind by a bullet that hits this surface type.")]
    [SerializeField]
    private WeaponImpactEffectDefinition m_BulletImpactEffectDefinition;
    public WeaponImpactEffectDefinition BulletImpactEffectDefinition
    {
        get { return m_BulletImpactEffectDefinition; }
    }

    [Tooltip("Decal left behind when a melee attack hits this surface type.")]
    [SerializeField]
    private WeaponImpactEffectDefinition m_MeleeImpactEffectDefinition;
    public WeaponImpactEffectDefinition MeleeImpactEffectDefinition
    {
        get { return m_MeleeImpactEffectDefinition; }
    }

    [Space(10)]
    [Header("Impact effect")]
    [Space(5)]
    [SerializeField]
    private PoolableObject m_ImpactEffectPrefab;
    public PoolableObject ImpactEffectPrefab
    {
        get { return m_ImpactEffectPrefab; }
    }

    [Space(10)]
    [Header("Footstep sounds")]
    [Space(5)]
    [SerializeField]
    private List<AudioClip> m_FootstepSounds;
    public List<AudioClip> FootstepSounds
    {
        get { return m_FootstepSounds; }
    }
}
