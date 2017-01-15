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

    public void PlaceDecal(RaycastHit hitInfo)
    {
        if (m_SurfaceType.DecalPrefab == null || m_SurfaceType.Decals.Count == 0)
            return;

        //Spawn the decal
        Vector3 decalPosition = hitInfo.point + (hitInfo.normal * 0.01f); //Offset the decal a bit from the wall
        Quaternion decalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
        GameObject go = GameObject.Instantiate(m_SurfaceType.DecalPrefab, decalPosition, decalRotation);

        //Set a random sprite (spawning a GameObject and referencing it as a SpriteRenderer wasn't something unity liked)
        SpriteRenderer decalRenderer = go.GetComponent<SpriteRenderer>();
        if (decalRenderer != null)
        {
            int randomDecalID = Random.Range(0, m_SurfaceType.Decals.Count - 1);
            decalRenderer.sprite = m_SurfaceType.Decals[randomDecalID];
        }
    }
}
