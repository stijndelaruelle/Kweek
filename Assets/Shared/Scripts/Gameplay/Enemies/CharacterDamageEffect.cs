using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamageEffect : MonoBehaviour
{
    [Header("Audio clips")]
    [Space(5)]
    [SerializeField]
    private AudioClip[] m_DamageClips;

    [SerializeField]
    private AudioClip m_DeathClip;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private IDamageableObject m_DamageableObject;

    [SerializeField]
    private AudioSource m_AudioSource;

    private void Start()
    {
        if (m_DamageableObject != null)
        {
            m_DamageableObject.DamageEvent += OnDamage;
            m_DamageableObject.DeathEvent += OnDeath;
        }
    }

    private void OnDestroy()
    {
        if (m_DamageableObject == null)
            return;

        m_DamageableObject.DamageEvent -= OnDamage;
        m_DamageableObject.DeathEvent -= OnDeath;
    }

    private void OnDamage(int removedHealth)
    {
        if (m_AudioSource == null || m_DamageClips == null || m_DamageClips.Length <= 0)
            return;

        if (m_AudioSource.isPlaying)
            return;

        if (m_DamageableObject.Health > 0)
        {
            //Determine the percentage of health this was, the more damage the higher the index in the sound array
            float percent = (float)removedHealth / (float)m_DamageableObject.MaxHealth;
            int soundIndex = Mathf.FloorToInt(percent * m_DamageClips.Length);

            m_AudioSource.clip = m_DamageClips[soundIndex];
            m_AudioSource.Play();
        }
    }

    private void OnDeath()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.clip = m_DeathClip;
            m_AudioSource.Play();
        }
    }
}