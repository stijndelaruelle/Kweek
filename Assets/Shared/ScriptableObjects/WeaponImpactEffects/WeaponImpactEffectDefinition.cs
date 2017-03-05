using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponImpactEffectDefinition : ScriptableObject
{
    [SerializeField]
    private List<Sprite> m_Decals;
    public List<Sprite> Decals
    {
        get { return m_Decals; }
    }

    [SerializeField]
    private List<AudioClip> m_AudioClips;
    public List<AudioClip> AudioClips
    {
        get { return m_AudioClips; }
    }
}