using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
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

    [Space(5)]
    [Header ("Required references")]
    [Space(10)]
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private AudioSource m_AudioSource;

    private void Awake()
    {
        //Set a random sprite
        if (m_SpriteRenderer != null && m_Decals.Count > 0)
        {
            int randomDecalID = 0;
            if (m_Decals.Count > 1) randomDecalID = Random.Range(0, m_Decals.Count);

            m_SpriteRenderer.sprite = m_Decals[randomDecalID];
        }

        //Play a random hit sound
        if (m_AudioSource != null && m_AudioClips.Count > 0)
        {
            int randomClipID = 0;
            if (m_AudioClips.Count > 1) randomClipID = Random.Range(0, m_AudioClips.Count);

            m_AudioSource.clip = m_AudioClips[randomClipID];
            m_AudioSource.Play();
        }
    }

}
