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

    [Header("HitBox")]
    [Space(5)]
    [SerializeField]
    private Collider m_Hitbox;

    [Tooltip("Only use when the animator is not on this Game Object")]
    [SerializeField]
    private HitboxMethodsForwarder m_HitBoxMethodForwarder;

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

    private void Start()
    {
        m_Hitbox.enabled = false;

        if (m_HitBoxMethodForwarder != null)
        {
            m_HitBoxMethodForwarder.HitboxEnableEvent += EnableHitbox;
            m_HitBoxMethodForwarder.HitboxDisableEvent += DisableHitbox;
        }
    }

    public override void Setup(List<Collider> ignoredColliders)
    {
        foreach (Collider collider in ignoredColliders)
        {
            Physics.IgnoreCollision(m_Hitbox, collider, true);
        }
    }

    private void OnDestroy()
    {
        if (m_HitBoxMethodForwarder != null)
        {
            m_HitBoxMethodForwarder.HitboxEnableEvent -= EnableHitbox;
            m_HitBoxMethodForwarder.HitboxDisableEvent -= DisableHitbox;
        }
    }

    private void Update()
    {
        HandleUsingCooldown();
    }

    public override void Use(Ray originalRay)
    {
        if (!CanUse())
            return;

        //Animation & Cooldown
        m_Animator.SetTrigger(m_TriggerName);
        m_UseCooldownTimer = m_UseCooldown;
    }

    public override bool CanUse()
    {
        return (m_UseCooldownTimer == 0.0f);
    }

    public override int GetAmmoUseage()
    {
        return 0;
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

    //Hitbox
    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;

        //Did we hit a damageableobject?
        IDamageableObject damageableObject = go.GetComponent<IDamageableObject>();

        if (damageableObject != null)
        {
            //Damage calculation
            damageableObject.Damage(m_Damage);
            return;
        }

        ////Did we hit a surface?
        //Vector3 position = collision.contacts[0].point;
        //Vector3 normal = collision.contacts[0].normal;

        //SurfaceType surfaceType = go.GetComponent<SurfaceType>();
        //if (surfaceType != null)
        //{
        //    surfaceType.SpawnMeleeImpactEffect(position, normal);
        //}

        ////Did we hit a rigidbody?
        //Rigidbody rigidBody = go.GetComponent<Rigidbody>();
        //if (rigidBody != null)
        //{
        //    rigidBody.AddForceAtPosition(-normal * m_ImpactForce, position);
        //}
    }

    //Animation events
    private void EnableHitbox()
    {
        m_Hitbox.enabled = true;
    }

    private void DisableHitbox()
    {
        m_Hitbox.enabled = false;
    }
}
