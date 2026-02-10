using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "SurfaceTypeDefinition", menuName = "Kweek/Surface Type Definition")]
    public class SurfaceTypeDefinition : ScriptableObject
    {
        [Header("Piercing damage reduction")]
        [Space(5)]
        [Tooltip("Amount of damage removed when hitting this surface type.")]
        [SerializeField]
        private int m_PiercingDamageFalloffFlat = 0;
        public int PiercingDamageFalloffFlat
        {
            get { return m_PiercingDamageFalloffFlat; }
        }

        [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
        [SerializeField]
        private int m_PiercingDamageFalloffPerUnit = 0;
        public int PiercingDamageFalloffPerUnit
        {
            get { return m_PiercingDamageFalloffPerUnit; }
        }

        [Space(10)]
        [Header("Piercing range reduction")]
        [Space(5)]
        [Tooltip("Amount of damage removed when hitting this surface type.")]
        [SerializeField]
        private float m_PiercingRangeFalloffFlat = 0.0f;
        public float PiercingRangeFalloffFlat
        {
            get { return m_PiercingRangeFalloffFlat; }
        }

        [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
        [SerializeField]
        private float m_PiercingRangeFalloffPerUnit = 0.0f;
        public float PiercingRangeFalloffPerUnit
        {
            get { return m_PiercingRangeFalloffPerUnit; }
        }

        [Space(10)]
        [Header("Impact effects")]
        [Space(5)]
        [SerializeField]
        private PoolableObject m_ImpactEffectPrefab = null;     //Should be simplified to clean up the inspector
        public PoolableObject ImpactEffectPrefab
        {
            get { return m_ImpactEffectPrefab; }
        }

        [Tooltip("Impact effect left behind by a bullet that hits this surface type.")]
        [SerializeField]
        private ImpactEffectDefinition m_BulletImpactEffectDefinition = null;
        public ImpactEffectDefinition BulletImpactEffectDefinition
        {
            get { return m_BulletImpactEffectDefinition; }
        }

        [Tooltip("Impact effect left behind when a melee attack hits this surface type.")]
        [SerializeField]
        private ImpactEffectDefinition m_MeleeImpactEffectDefinition = null;
        public ImpactEffectDefinition MeleeImpactEffectDefinition
        {
            get { return m_MeleeImpactEffectDefinition; }
        }

        [Tooltip("Impact effect left behind when a character hits this surface type.")]
        [SerializeField]
        private ImpactEffectDefinition m_CharacterImpactEffectDefinition = null;
        public ImpactEffectDefinition CharacterImpactEffectDefinition
        {
            get { return m_CharacterImpactEffectDefinition; }
        }

        [Space(10)]
        [Header("Footstep sounds")]
        [Space(5)]
        [SerializeField]
        private List<AudioClip> m_FootstepSounds = null;
        public List<AudioClip> FootstepSounds
        {
            get { return m_FootstepSounds; }
        }
    }
}