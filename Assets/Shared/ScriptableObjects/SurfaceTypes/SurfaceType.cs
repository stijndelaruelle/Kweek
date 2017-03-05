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

    public List<AudioClip> FootstepSounds
    {
        get { return m_SurfaceType.FootstepSounds; }
    }

    public void SpawnBulletImpactEffect(RaycastHit hitInfo)
    {
        if (m_SurfaceType.WeaponImpactEffectPrefab == null)
            return;

        ObjectPool pool = ObjectPoolManager.Instance.GetPool(m_SurfaceType.WeaponImpactEffectPrefab);

        if (pool != null && pool.IsPoolType<WeaponImpactEffect>())
        {
            WeaponImpactEffect bulletImpactEffect = pool.GetAvailableObject() as WeaponImpactEffect;
            if (bulletImpactEffect != null)
            {
                Vector3 decalPosition = hitInfo.point + (hitInfo.normal * 0.01f); //Offset the decal a bit from the wall
                Quaternion decalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

                bulletImpactEffect.transform.position = decalPosition;
                bulletImpactEffect.transform.rotation = decalRotation;
                bulletImpactEffect.transform.SetParent(transform);

                bulletImpactEffect.InitializeWeaponImpactEffect(m_SurfaceType.BulletImpactEffectDefinition);
                bulletImpactEffect.Activate();
            }
        }
    }

    public GameObject SpawnImpactEffect(Vector3 position)
    {
        if (m_SurfaceType.ImpactEffectPrefab == null)
            return null;

        ObjectPool pool = ObjectPoolManager.Instance.GetPool(m_SurfaceType.ImpactEffectPrefab);

        if (pool != null)
        {
            PoolableObject impactEffect = pool.ActivateAvailableObject();
            if (impactEffect != null)
            {
                impactEffect.transform.position = position;
                impactEffect.transform.rotation = m_SurfaceType.ImpactEffectPrefab.transform.rotation;
                impactEffect.transform.SetParent(transform);

                return impactEffect.gameObject;
            }
        }

        return null;
    }
}
