using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTypeDefinition : ScriptableObject
{
    [Tooltip("Amount of damage removed when moving trough 1 unit of this surface type.")]
    [SerializeField]
    private int m_PiercingDamageFalloff;
    public int PiercingDamageFalloff
    {
        get { return m_PiercingDamageFalloff; }
    }

    [Tooltip("Decal left behind by a bullet that hits this surface type.")]
    [SerializeField]
    private GameObject m_DecalPrefab; //Change into a decal pool later
    public GameObject DecalPrefab
    {
        get { return m_DecalPrefab; }
    }

    [SerializeField]
    private List<Sprite> m_Decals;
    public List<Sprite> Decals
    {
        get { return m_Decals; }
    }

    //Hit particle
    //Hit sound
    //Walk sound
}
