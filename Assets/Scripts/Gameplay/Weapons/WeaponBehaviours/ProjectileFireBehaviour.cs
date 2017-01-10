using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFireBehaviour : IFireBehaviour
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

    [SerializeField]
    private PlayerController m_PlayerController;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private string m_TriggerName = "FireTrigger";

    private void Update()
    {
        HandleShootingCooldown();
    }

    public override void Fire()
    {
        PhysicalProjectile projectile = GameObject.Instantiate<PhysicalProjectile>(m_Projectile, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation);

        if (projectile != null)
        {
            Ray centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 targetPosition = centerRay.origin + (centerRay.direction * 1000.0f); //A position far away
            Vector3 direction = (targetPosition - m_ProjectileSpawn.position).normalized;

            projectile.Fire(m_PlayerController.CurrentVelocity, direction);

            //Animation & Cooldown
            m_Animator.SetTrigger(m_TriggerName);
            m_ShootCooldownTimer = m_ShootCooldown;
        }
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

    public override bool CanFire()
    {
        return (m_ShootCooldownTimer == 0.0f);
    }

    public override int GetAmmoUseage()
    {
        return m_AmmoUseage;
    }
}
