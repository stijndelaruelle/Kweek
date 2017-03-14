using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotFireBehaviour : IWeaponUseBehaviour
{
    [SerializeField]
    private float m_ShootCooldown = 0.0f;
    private float m_ShootCooldownTimer;

    [Tooltip("How much ammo does chargeing the gun for 1 second take")]
    [SerializeField]
    private int m_AmmoUseage = 1;
    private float m_AmmoTimer = 0.0f;

    [SerializeField]
    private ChargeableProjectile m_ProjectilePrefab;
    private ChargeableProjectile m_CurrentProjectile;

    [SerializeField]
    private Transform m_ProjectileSpawn;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private string m_TriggerName = "FireTrigger";

    private List<Collider> m_IgnoredColliders;
    private Ray m_LastUsedRay; //To fire when fully charged

    //Events
    public override event AmmoUseDelegate AmmoUseEvent;

    public override void Setup(List<Collider> ignoredColliders)
    {
        m_IgnoredColliders = ignoredColliders;
    }

    private void Update()
    {
        HandleShootingCooldown();
    }

    public override bool Use(Ray originalRay)
    {
        m_LastUsedRay = originalRay;

        //Create new projectile
        if (m_CurrentProjectile == null)
        {
            m_CurrentProjectile = GameObject.Instantiate<ChargeableProjectile>(m_ProjectilePrefab, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation, m_ProjectileSpawn);
            m_CurrentProjectile.FullChargedEvent += OnFullyCharged;
        }

        //Charge the projectile
        m_CurrentProjectile.Charge(Time.deltaTime);

        //TODO: Detonate projectile (if it's already flying)

        //Ammo
        m_AmmoTimer += Time.deltaTime;

        float ammoUseFreq = (1.0f / m_AmmoUseage);
        if (m_AmmoTimer >= ammoUseFreq)
        {
            m_AmmoTimer -= ammoUseFreq;

            if (AmmoUseEvent != null)
                AmmoUseEvent(1);
        }

        return true;
    }

    public override bool StopUse(Ray originalRay)
    {
        if (m_CurrentProjectile == null)
            return false;

        if (m_CurrentProjectile.CanFire() == false)
            return false;

        m_CurrentProjectile.transform.parent = null;
        bool success = m_CurrentProjectile.Fire(originalRay.direction, Vector3.zero);

        if (success == false)
            return false;

        //Animation & Cooldown
        if (m_Animator != null)
        {
            m_Animator.SetTrigger(m_TriggerName);
        }
        
        m_ShootCooldownTimer = m_ShootCooldown;

        m_CurrentProjectile.FullChargedEvent -= OnFullyCharged;
        m_CurrentProjectile = null;

        return true;
    }

    private void HandleShootingCooldown()
    {
        if (m_ShootCooldownTimer > 0.0f)
        {
            m_ShootCooldownTimer -= Time.deltaTime;

            if (m_ShootCooldownTimer <= 0.0f)
            {
                m_ShootCooldownTimer = 0.0f;
            }
        }
    }

    public override bool CanUse()
    {
        return (m_ShootCooldownTimer == 0.0f);
    }

    private void OnFullyCharged()
    {
        StopUse(m_LastUsedRay);
    }
}
