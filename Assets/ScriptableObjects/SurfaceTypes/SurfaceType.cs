using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceType : MonoBehaviour
{
    [SerializeField]
    private SurfaceTypeDefinition m_SurfaceType;

    public int PiercingDamageFalloffFlat
    {
        get { return m_SurfaceType.PiercingDamageFalloffFlat; }
    }

    public int PiercingDamageFalloffPerUnit
    {
        get { return m_SurfaceType.PiercingDamageFalloffPerUnit; }
    }

    public float PiercingRangeFalloffFlat
    {
        get { return m_SurfaceType.PiercingRangeFalloffFlat; }
    }

    public float PiercingRangeFalloffPerUnit
    {
        get { return m_SurfaceType.PiercingRangeFalloffPerUnit; }
    }


    public void PlaceDecal(RaycastHit hitInfo)
    {
        if (m_SurfaceType.ImpactEffectPrefab == null)
            return;

        //Spawn the decal (pool this later
        Vector3 decalPosition = hitInfo.point + (hitInfo.normal * 0.01f); //Offset the decal a bit from the wall
        Quaternion decalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

        Instantiate(m_SurfaceType.ImpactEffectPrefab, decalPosition, decalRotation);
    }
}
