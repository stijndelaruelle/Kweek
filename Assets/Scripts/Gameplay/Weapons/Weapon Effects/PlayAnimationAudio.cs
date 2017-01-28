using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayAnimationAudio : MonoBehaviour
{
    [SerializeField]
    private AudioMixerGroup m_Mixer;

    [SerializeField]
    private int m_MaxChannels;

    private List<AudioSource> m_AudioSources;
    private int m_CurrentChannel;

    private void Awake()
    {
        m_AudioSources = new List<AudioSource>();

        for (int i = 0; i < m_MaxChannels; ++i)
        {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.outputAudioMixerGroup = m_Mixer;
            newAudioSource.playOnAwake = false;
            newAudioSource.loop = false;

            m_AudioSources.Add(newAudioSource);
        }
    }

    public void PlayAudio(Object objAudioClip)
    {
        AudioClip audioClip = (objAudioClip as AudioClip);

        if (audioClip == null)
            return;

        //Find available channel
        m_AudioSources[m_CurrentChannel].clip = audioClip;
        m_AudioSources[m_CurrentChannel].Play();

        m_CurrentChannel += 1;
        if (m_CurrentChannel >= m_AudioSources.Count) m_CurrentChannel = 0;
    }
}
