using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanFireBehaviour : IFireBehaviour
{
    [Header("Damage")]
    [Space(5)]
    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private AnimationCurve m_DamageFalloff;

    [Tooltip("Max distance that the projectile will fly.")]
    [SerializeField]
    private float m_Range;

    [SerializeField]
    private float m_ShootCooldown = 0.0f;
    private float m_ShootCooldownTimer;

    [Tooltip("How much ammo does firing the gun 1 time take.")]
    [SerializeField]
    private int m_AmmoUseage = 1;

    [Header("Spread")]
    [Space(5)]
    [SerializeField]
    private int m_NumberOfProjectiles;

    [Tooltip("Max spread at max range.")]
    [SerializeField]
    private float m_Spread;

    [Space(10)]
    [Header("Animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private string m_TriggerName = "FireTrigger";

    //[SerializeField]
    //private bool m_Piercing;
    //Piercing falloff is determined by the material we've hit (think counter strike)

    private void Update()
    {
        HandleShootingCooldown();
    }

    public override void Fire()
    {
        if (!CanFire())
            return;

        Ray centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        Vector3 forward = centerRay.direction;
        Vector3 right = new Vector3(-forward.z, 0.0f, forward.x);
        Vector3 up = Vector3.Cross(right, forward);

        for (int i = 0; i < m_NumberOfProjectiles; ++i)
        {
            Ray ray = centerRay;
            float range = m_Range;

            if (m_Spread > 0.0f)
            {
                Vector3 maxPosition = centerRay.direction * m_Range;

                maxPosition += right * UnityEngine.Random.Range(-m_Spread, m_Spread);
                maxPosition += up * UnityEngine.Random.Range(-m_Spread, m_Spread);

                ray.direction = maxPosition.normalized;
                range = maxPosition.magnitude;
            }

            FireRay(ray, range);
        }

        //Animation & Cooldown
        m_Animator.SetTrigger(m_TriggerName);
        m_ShootCooldownTimer = m_ShootCooldown;
    }

    public void FireRay(Ray ray, float range)
    {
        //Fire a single ray (get only the first target)
        RaycastHit hitInfo;
        bool succes = Physics.Raycast(ray, out hitInfo, range);

        if (!succes)
            return;

        GameObject go = hitInfo.collider.gameObject;

        //Did we hit a damageableobject?
        IDamageableObject damageableObject = go.GetComponent<IDamageableObject>();

        if (damageableObject != null)
        {
            //Damage calculation
            int damage = CalculateDamage(ray.origin, hitInfo.point, range);

            damageableObject.Damage(damage);
            return;
        }

        //Did we hit a surface?
        SurfaceType surfaceType = go.GetComponent<SurfaceType>();
        if (surfaceType != null)
        {
            Vector3 decalPosition = hitInfo.point + (hitInfo.normal * 0.01f); //Offset the decal a bit from the wall
            Quaternion decalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

            GameObject.Instantiate(surfaceType.Decal, decalPosition, decalRotation);
        }
    }

    private List<IDamageableObject> FirePiercingRay(Ray ray, out List<RaycastHit> hitInfo)
    {
        //------------------
        // TODO: Piercing damage (RaycastAll)
        //------------------
        hitInfo = new List<RaycastHit>();
        return null;



        //Fire a single ray (get all the targets)
        //RaycastHit[] hitInfoArr = Physics.RaycastAll(centerRay, m_Range);

        //if (hitInfoArr.Length > 0)
        //{
        //for (int i = 0; i < hitInfoArr.Length; ++i)
        //{
        //Debug.Log("COULD HAVE HIT: " + hitInfoArr[i].collider.gameObject.name);

        //        IDamageableObject damageableObject = hitInfo[i].collider.gameObject.GetComponent<IDamageableObject>();

        //        if (damageableObject != null)
        //        {
        //            damageableObject.Damage(m_Damage);
        //        }
        //}
        //}
    }

    private int CalculateDamage(Vector3 start, Vector3 end, float range)
    {
        float distance = (end - start).magnitude;
        float normDistance = distance / range;
        float damagePercentage = m_DamageFalloff.Evaluate(normDistance);

        Debug.Log("Damage percentage: " + damagePercentage + " = " + (m_Damage * damagePercentage));
        return (int)(m_Damage * damagePercentage);
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
