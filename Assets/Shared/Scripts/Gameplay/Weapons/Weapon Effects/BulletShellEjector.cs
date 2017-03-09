using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShellEjector : MonoBehaviour
{
    [SerializeField]
    private Weapon m_Weapon;

    //Should be simplified to clean up the inspector
    [SerializeField]
    private BulletShell m_BulletShellPrefab;

    [SerializeField]
    private BulletShellDefinition m_BulletShellDefinition;

    [SerializeField]
    private float m_EjectSpeed;

    [SerializeField]
    private float m_Delay; //Weapons like the shotgun eject some time after firing
    private float m_DelayTimer = 0.0f;

    private void Start()
    {
        if (m_Weapon != null)
            m_Weapon.WeaponUseEvent += OnWeaponFire;
    }

    private void OnDestroy()
    {
        if (m_Weapon != null)
            m_Weapon.WeaponUseEvent -= OnWeaponFire;
    }

    private void Update()
    {
        if (m_DelayTimer > 0.0f)
        {
            m_DelayTimer -= Time.deltaTime;

            if (m_DelayTimer <= 0.0f)
            {
                m_DelayTimer = 0.0f;
                EjectShell();
            }
        }
    }

    private void OnWeaponFire()
    {
        if (m_Delay > 0.0f)
        {
            m_DelayTimer = m_Delay;
            return;
        }

        EjectShell();
    }

    private void EjectShell()
    {
        ObjectPool pool = ObjectPoolManager.Instance.GetPool(m_BulletShellPrefab);

        if (pool != null && pool.IsPoolType<BulletShell>())
        {
            BulletShell bulletShell = pool.GetAvailableObject() as BulletShell;
            if (bulletShell != null)
            {
                bulletShell.transform.position = transform.position;
                bulletShell.transform.rotation = transform.rotation * m_BulletShellPrefab.transform.rotation;
                bulletShell.transform.SetParent(transform);

                bulletShell.InitializeBulletShell(m_BulletShellDefinition);
                bulletShell.Activate();
                bulletShell.Eject(transform.right * m_EjectSpeed, m_Weapon.OwnerCollider);
            }
        }
        else
        {
            Debug.LogWarning("No bullet shell pool found for " + m_BulletShellPrefab.name);
        }
    }
}
