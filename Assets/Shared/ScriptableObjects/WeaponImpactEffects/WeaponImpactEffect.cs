using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class WeaponImpactEffect : PoolableObject
{
    [SerializeField]
    private WeaponImpactEffectDefinition m_Definition;

    [Space(5)]
    [Header ("Required references")]
    [Space(10)]
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private AudioSource m_AudioSource;

    public void InitializeWeaponImpactEffect(WeaponImpactEffectDefinition definition)
    {
        m_Definition = definition;
    }

    //PoolableObject
    public override void Initialize()
    {

    }

    public override void Activate()
    {
        //Set a random sprite
        if (m_SpriteRenderer != null && m_Definition.Decals.Count > 0)
        {
            int randomDecalID = 0;
            if (m_Definition.Decals.Count > 1) randomDecalID = Random.Range(0, m_Definition.Decals.Count);

            m_SpriteRenderer.enabled = true;
            m_SpriteRenderer.sprite = m_Definition.Decals[randomDecalID];
        }

        //Play a random hit sound
        if (m_AudioSource != null && m_Definition.AudioClips.Count > 0)
        {
            int randomClipID = 0;
            if (m_Definition.AudioClips.Count > 1) randomClipID = Random.Range(0, m_Definition.AudioClips.Count);

            m_AudioSource.clip = m_Definition.AudioClips[randomClipID];
            m_AudioSource.Play();
        }
    }

    public override void Deactivate()
    {
        m_SpriteRenderer.enabled = false;
        m_AudioSource.Stop();
    }

    public override bool IsAvailable()
    {
        return true;
    }
}
