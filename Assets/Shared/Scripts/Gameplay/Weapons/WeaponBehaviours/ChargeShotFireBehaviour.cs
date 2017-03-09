using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotFireBehaviour : IWeaponUseBehaviour
{
    [SerializeField]
    private float m_ShootCooldown = 0.0f;
    private float m_ShootCooldownTimer;

    [Tooltip("How much ammo does changing the gun for 1 second take")]
    [SerializeField]
    private int m_AmmoUseage = 1;
    private float m_ChargeTimer = 0.0f;

    [MinMaxRange(5f, 10f)]
    // inspector slider can move between these values
    public MinMaxRange speed;

    [SerializeField]
    private PhysicalProjectile m_ProjectilePrefab;
    private PhysicalProjectile m_CurrentProjectile;

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
        HandleCharging();
        HandleShootingCooldown();
    }

    public override void Use(Ray originalRay)
    {
        //Create new projectile
        if (m_CurrentProjectile != null)
        {
            m_CurrentProjectile = GameObject.Instantiate<PhysicalProjectile>(m_ProjectilePrefab, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation);
            m_ChargeTimer = 0.0f;
        }

        //Detonate projectile
    }

    public override void StopUse(Ray originalRay)
    {
        if (m_CurrentProjectile == null)
            return;

        //Look at this at a later stage. Controllers have undergone huge changes.
        m_CurrentProjectile.Fire(m_ProjectileSpawn.forward, Vector3.zero);

        //Animation & Cooldown
        m_Animator.SetTrigger(m_TriggerName);
        m_ShootCooldownTimer = m_ShootCooldown;
    }

    private void HandleCharging()
    {
        if (m_CurrentProjectile == null)
            return;

        //Make bigger
        //m_CurrentProjectile.transform.localScale = new Vector3()

        //Increase damage
        m_ChargeTimer += Time.deltaTime;

        //Use ammo callback?

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
