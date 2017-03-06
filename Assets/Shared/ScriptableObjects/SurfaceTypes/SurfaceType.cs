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


    public GameObject SpawnBulletImpactEffect(Vector3 position, Vector3 normal)
    {
        return SpawnImpactEffect(m_SurfaceType.BulletImpactEffectDefinition, position, normal);
    }

    public GameObject SpawnMeleeImpactEffect(Vector3 position, Vector3 normal)
    {
        return SpawnImpactEffect(m_SurfaceType.MeleeImpactEffectDefinition, position, normal);
    }

    public GameObject SpawnCharacterImpactEffect(Vector3 position)
    {
        return SpawnImpactEffect(m_SurfaceType.CharacterImpactEffectDefinition, position, Vector3.zero);
    }

    private GameObject SpawnImpactEffect(ImpactEffectDefinition impactDefinition, Vector3 position, Vector3 normal)
    {
        if (m_SurfaceType.ImpactEffectPrefab == null)
            return null;

        ObjectPool pool = ObjectPoolManager.Instance.GetPool(m_SurfaceType.ImpactEffectPrefab);

        if (pool != null && pool.IsPoolType<ImpactEffect>())
        {
            ImpactEffect bulletImpactEffect = pool.GetAvailableObject() as ImpactEffect;
            if (bulletImpactEffect != null)
            {
                Vector3 decalPosition = position + (normal * 0.01f); //Offset the decal a bit from the wall
                Quaternion decalRotation = Quaternion.LookRotation(normal, Vector3.up);

                bulletImpactEffect.transform.position = decalPosition;
                bulletImpactEffect.transform.rotation = decalRotation;
                bulletImpactEffect.transform.SetParent(transform);

                bulletImpactEffect.InitializeImpactEffect(impactDefinition);
                bulletImpactEffect.Activate();

                return bulletImpactEffect.gameObject;
            }
        }

        return null;
    }
}
