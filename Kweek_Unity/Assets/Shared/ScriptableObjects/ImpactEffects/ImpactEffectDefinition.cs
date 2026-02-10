using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "ImpactEffectDefinition", menuName = "Kweek/Impact Effect Definition")]
    public class ImpactEffectDefinition : ScriptableObject
    {
        [SerializeField]
        private List<Sprite> m_Decals = null;
        public List<Sprite> Decals
        {
            get { return m_Decals; }
        }

        [SerializeField]
        private List<AudioClip> m_AudioClips = null;
        public List<AudioClip> AudioClips
        {
            get { return m_AudioClips; }
        }
    }
}