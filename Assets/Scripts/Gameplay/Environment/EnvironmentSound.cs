using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSound : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_AudioClips;
    public List<AudioClip> AudioClips
    {
        get { return m_AudioClips; }
    }

    [Space(5)]
    [Header("Required references")]
    [Space(10)]

    [SerializeField]
    private AudioSource m_AudioSource;

    private void Awake()
    {
        //Play a random hit sound
        if (m_AudioSource != null && m_AudioClips.Count > 0)
        {
            int randomClipID = 0;
            if (m_AudioClips.Count > 1) randomClipID = Random.Range(0, m_AudioClips.Count);

            m_AudioSource.clip = m_AudioClips[randomClipID];
            m_AudioSource.Play();

            StartCoroutine(DestroyRoutine());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //Should be pooled in the future
    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(m_AudioSource.clip.length);
        Destroy(this.gameObject);
    }

}
