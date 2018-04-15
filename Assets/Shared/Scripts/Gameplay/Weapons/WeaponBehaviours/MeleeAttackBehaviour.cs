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
    [Tooltip("I know it's a sphere and not a box but 'Hitbox' is simply a more commonly used term")]
    [Space(5)]
    [SerializeField]
    private float m_HitboxRadius;

    [SerializeField]
    private LayerMask m_HitboxLayerMask;

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

    private List<Collider> m_IgnoredColliders;

    //Events
    public override event AmmoUseDelegate AmmoUseEvent;

    public override void Setup(List<Collider> ignoredColliders)
    {
        m_IgnoredColliders = ignoredColliders;
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
        HandleSwing();

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

    private void HandleSwing()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HitboxRadius, m_HitboxLayerMask.value);

        //For all targets in my radius
        for (int i = 0; i < colliders.Length; ++i)
        {
            Collider other = colliders[i];

            //Don't hit ourselves
            if (m_IgnoredColliders.Contains(other) == false)
            {
                IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
                if (damageableObject != null)
                {
                    damageableObject = damageableObject.GetMainDamageableObject();

                    //Deal damage to them
                    damageableObject.Damage(m_Damage);
                }
            } 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_HitboxRadius);
    }
}
