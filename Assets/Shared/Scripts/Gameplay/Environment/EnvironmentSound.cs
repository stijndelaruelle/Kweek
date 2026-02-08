using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    public class EnvironmentSound : PoolableObject
    {
        [SerializeField]
        private List<AudioClip> m_AudioClips = null;
        public List<AudioClip> AudioClips
        {
            get { return m_AudioClips; }
        }

        [Space(5)]
        [Header("Required references")]
        [Space(10)]

        [SerializeField]
        private AudioSource m_AudioSource = null;

        //PoolableObject
        public override void Initialize()
        {

        }

        public override void Activate()
        {
            //Play a random hit sound
            if (m_AudioSource != null && m_AudioClips.Count > 0)
            {
                int randomClipID = 0;
                if (m_AudioClips.Count > 1) randomClipID = Random.Range(0, m_AudioClips.Count);

                m_AudioSource.clip = m_AudioClips[randomClipID];
                m_AudioSource.Play();
            }
        }

        public override void Deactivate()
        {
            m_AudioSource.Stop();
        }

        public override bool IsAvailable()
        {
            return true;
        }
    }
}