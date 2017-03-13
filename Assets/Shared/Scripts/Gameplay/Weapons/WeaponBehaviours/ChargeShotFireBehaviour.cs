﻿using System.Collections;
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
        //Create new projectile
        if (m_CurrentProjectile == null)
        {
            m_CurrentProjectile = GameObject.Instantiate<ChargeableProjectile>(m_ProjectilePrefab, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation, m_ProjectileSpawn);
            m_CurrentProjectile.StartCharging();
        }

        //TODO: Detonate projectile

        return true;
    }

    public override bool StopUse(Ray originalRay)
    {
        if (m_CurrentProjectile == null)
            return false;

        if (m_CurrentProjectile.CanFire() == false)
            return false;

        //Look at this at a later stage. Controllers have undergone huge changes.
        m_CurrentProjectile.transform.parent = null;
        bool success = m_CurrentProjectile.Fire(m_ProjectileSpawn.forward, Vector3.zero);

        if (success == false)
            return false;

        //Animation & Cooldown
        if (m_Animator != null)
        {
            m_Animator.SetTrigger(m_TriggerName);
        }
        
        m_ShootCooldownTimer = m_ShootCooldown;
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

    public override int GetAmmoUseage()
    {
        return m_AmmoUseage;
    }
}