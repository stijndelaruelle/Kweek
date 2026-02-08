using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "BulletShellDefinition", menuName = "Kweek/Bullet Shell Definition")]
    public class BulletShellDefinition : ScriptableObject
    {
        [Header("General")]
        [SerializeField]
        private List<AudioClip> m_AudioClips = null;
        public List<AudioClip> AudioClips
        {
            get { return m_AudioClips; }
        }

        [SerializeField]
        private bool m_DelayedDecouple = false;
        public bool DelayedDecouple
        {
            get { return m_DelayedDecouple; }
        }

        [Header("Collider")]
        [SerializeField]
        private float m_Radius = 0.0f;
        public float Radius
        {
            get { return m_Radius; }
        }

        [SerializeField]
        private float m_Height = 0.0f;
        public float Height
        {
            get { return m_Height; }
        }

        [Header("Visuals")]
        [SerializeField]
        private Mesh m_Mesh = null;
        public Mesh Mesh
        {
            get { return m_Mesh; }
        }

        [SerializeField]
        private Material m_Material = null;
        public Material Material
        {
            get { return m_Material; }
        }

        [SerializeField]
        private Vector3 m_MeshScale = Vector3.zero;
        public Vector3 MeshScale
        {
            get { return m_MeshScale; }
        }
    }
}