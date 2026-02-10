using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Kweek
{
    public class PlayAnimationAudio : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerGroup m_Mixer = null;

        [SerializeField]
        private int m_MaxChannels = 0;

        [SerializeField]
        private AudioClip m_PresetClip = null;

        [SerializeField]
        private bool m_3D = false;

        [SerializeField]
        private float m_MinDistance = 1.0f;

        private List<AudioSource> m_AudioSources = null;
        private int m_CurrentChannel = 0;

        private void Awake()
        {
            m_AudioSources = new List<AudioSource>();

            for (int i = 0; i < m_MaxChannels; ++i)
            {
                AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
                newAudioSource.outputAudioMixerGroup = m_Mixer;
                newAudioSource.playOnAwake = false;
                newAudioSource.loop = false;
                if (m_3D)
                {
                    newAudioSource.spatialBlend = 1.0f;
                    newAudioSource.minDistance = m_MinDistance;
                }

                m_AudioSources.Add(newAudioSource);
            }
        }

        public void PlayPresetAudio()
        {
            PlayAudioClip(m_PresetClip);
        }

        public void PlayAudio(Object objAudioClip)
        {
            AudioClip audioClip = (objAudioClip as AudioClip);
            PlayAudioClip(audioClip);
        }

        private void PlayAudioClip(AudioClip audioClip)
        {
            if (audioClip == null)
                return;

            //Find available channel
            m_AudioSources[m_CurrentChannel].clip = audioClip;
            m_AudioSources[m_CurrentChannel].Play();

            m_CurrentChannel += 1;
            if (m_CurrentChannel >= m_AudioSources.Count) m_CurrentChannel = 0;
        }
    }
}