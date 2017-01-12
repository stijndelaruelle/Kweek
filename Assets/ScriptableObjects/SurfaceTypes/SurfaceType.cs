using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceType : MonoBehaviour
{
    [SerializeField]
    private SurfaceTypeDefinition m_SurfaceType;
  
    public int PiercingDamageFalloff
    {
        get { return m_SurfaceType.PiercingDamageFalloff; }
    }

    public GameObject Decal
    {
        get { return m_SurfaceType.Decal; }
    }
}
