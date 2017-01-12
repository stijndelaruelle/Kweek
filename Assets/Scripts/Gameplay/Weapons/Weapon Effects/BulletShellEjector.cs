using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShellEjector : MonoBehaviour
{
    [SerializeField]
    private Weapon m_Weapon;

    [SerializeField]
    private BulletShell m_BulletShell;

    [SerializeField]
    private float m_EjectSpeed;

    [SerializeField]
    private float m_Delay; //Weapons like the shotgun eject some time after firing
    private float m_DelayTimer = 0.0f;

    private void Start()
    {
        if (m_Weapon != null)
            m_Weapon.WeaponFireEvent += OnWeaponFire;
    }

    private void OnDestroy()
    {
        if (m_Weapon != null)
            m_Weapon.WeaponFireEvent -= OnWeaponFire;
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
        BulletShell bulletShell = GameObject.Instantiate(m_BulletShell, transform.position, transform.rotation);

        bulletShell.transform.parent = transform;

        bulletShell.Eject(transform.right * m_EjectSpeed);
    }
}
