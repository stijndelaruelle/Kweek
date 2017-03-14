using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFireBehaviour : IWeaponUseBehaviour
{
    [SerializeField]
    private float m_ShootCooldown = 0.0f;
    private float m_ShootCooldownTimer;

    [Tooltip("How much ammo does firing the gun 1 time take.")]
    [SerializeField]
    private int m_AmmoUseage = 1;

    [SerializeField]
    private PhysicalProjectile m_Projectile;

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
        PhysicalProjectile projectile = GameObject.Instantiate<PhysicalProjectile>(m_Projectile, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation);

        if (projectile != null)
        {
            //Vector3 targetPosition = originalRay.origin + (originalRay.direction * 1000.0f); //A position far away
            //Vector3 direction = (targetPosition - m_ProjectileSpawn.position).normalized;

            //Look at this at a later stage. Controllers have undergone huge changes.
            projectile.Fire(m_ProjectileSpawn.forward, Vector3.zero);

            //Animation & Cooldown
            m_Animator.SetTrigger(m_TriggerName);
            m_ShootCooldownTimer = m_ShootCooldown;
            return true;
        }

        return false;
    }

    public override bool StopUse(Ray originalRay)
    {
        //This weapon has no need for this, however it's still generic enough to be included in the interface.
        return false;
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

    public override int GetAmmoUseage()
    {
        return m_AmmoUseage;
    }
}
