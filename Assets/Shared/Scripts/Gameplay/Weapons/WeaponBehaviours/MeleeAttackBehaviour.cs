using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehaviour : IWeaponUseBehaviour
{
    [Header("General")]
    [Space(5)]
    [SerializeField]
    private float m_UseCooldown = 0.0f;
    private float m_UseCooldownTimer;

    [Tooltip("How much ammo does using the weapon 1 time take.")]
    [SerializeField]
    private int m_AmmoUseage = 0;

    [Header("HitBox")]
    [Space(5)]

    [Space(10)]
    [Header("Damage")]
    [Space(5)]
    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private float m_ImpactForce;

    [Space(10)]
    [Header("Animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private string m_TriggerName = "FireTrigger";

    //Events
    public override event AmmoUseDelegate AmmoUseEvent;

    public override void Setup(List<Collider> ignoredColliders)
    {
    }

    private void Update()
    {
        HandleUsingCooldown();
    }

    public override bool Use(Ray originalRay)
    {
        if (!CanUse())
            return false;

        //Animation & Cooldown
        m_Animator.SetTrigger(m_TriggerName);
        m_UseCooldownTimer = m_UseCooldown;

        //Use ammo (avoids warning)
        if (AmmoUseEvent != null)
            AmmoUseEvent(m_AmmoUseage);

        //Actually do damage
        //TODO

        return true;
    }

    public override bool StopUse(Ray originalRay)
    {
        //This weapon has no need for this, however it's still generic enough to be included in the interface.
        return false;
    }

    public override bool CanUse()
    {
        return (m_UseCooldownTimer == 0.0f);
    }

    private void HandleUsingCooldown()
    {
        if (m_UseCooldownTimer > 0.0f)
        {
            m_UseCooldownTimer -= Time.deltaTime;

            if (m_UseCooldownTimer <= 0.0f)
            {
                m_UseCooldownTimer = 0.0f;
            }
        }
    }


    //Animation events
    private void EnableHitbox()
    {
        //The animation tells us when we can do damage
        //m_Hitbox.enabled = true;
    }

    private void DisableHitbox()
    {
        //The animation tells us when we stop doing damage
        //m_Hitbox.enabled = false;
    }
}
