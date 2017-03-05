using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShellDefinition : ScriptableObject
{
    [Header("General")]
    [SerializeField]
    private List<AudioClip> m_AudioClips;
    public List<AudioClip> AudioClips
    {
        get { return m_AudioClips; }
    }

    [SerializeField]
    private bool m_DelayedDecouple;
    public bool DelayedDecouple
    {
        get { return m_DelayedDecouple; }
    }

    [Header("Collider")]
    [SerializeField]
    private float m_Radius;
    public float Radius
    {
        get { return m_Radius; }
    }

    [SerializeField]
    private float m_Height;
    public float Height
    {
        get { return m_Height; }
    }

    [Header("Visuals")]
    [SerializeField]
    private Mesh m_Mesh;
    public Mesh Mesh
    {
        get { return m_Mesh; }
    }

    [SerializeField]
    private Material m_Material;
    public Material Material
    {
        get { return m_Material; }
    }

    [SerializeField]
    private Vector3 m_MeshScale;
    public Vector3 MeshScale
    {
        get { return m_MeshScale; }
    }
}
