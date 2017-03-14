using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitScanFireBehaviour : IWeaponUseBehaviour
{
    [Header("General")]
    [Space(5)]
    [SerializeField]
    private float m_ShootCooldown = 0.0f;
    private float m_ShootCooldownTimer;

    [Tooltip("How much ammo does firing the gun 1 time take.")]
    [SerializeField]
    private int m_AmmoUseage = 1;

    [Space(10)]
    [Header("Damage")]
    [Space(5)]
    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private float m_ImpactForce;

    [SerializeField]
    private AnimationCurve m_DamageFalloff;

    [SerializeField]
    private bool m_Piercing;

    [Tooltip("Max distance that the projectile will fly.")]
    [SerializeField]
    private float m_Range;

    [Space(10)]
    [Header("Recoil")]
    [Space(5)]
    [SerializeField]
    private AnimationCurve m_RecoilPattern; //TODO: Create custom inspector because the curve editor can't handle multiple keyframes at the same time.

    [Tooltip("Adds randomisation in both directions")]
    [SerializeField]
    private AnimationCurve m_RecoilSpreadRate;

    [Tooltip("Time = number of bullets fired, Value = Time it takes to recoved entirely + Shoot Cooldown")]
    [SerializeField]
    private AnimationCurve m_RecoilCooldownRate;
    private float m_RecoilCooldownTimer;

    private int m_CurrentRecoilBullet = 0;

    [Space(10)]
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

    private List<Collider> m_IgnoredColliders;

    //Events
    public override event AmmoUseDelegate AmmoUseEvent;

    public override void Setup(List<Collider> ignoredColliders)
    {
        m_IgnoredColliders = ignoredColliders;
    }

    private void Update()
    {
        HandleShootingCooldown();
        HandleRecoilCooldown();
    }

    public override bool Use(Ray originalRay)
    {
        if (!CanUse())
            return false;

        Vector3 forward = originalRay.direction;
        Vector3 right = new Vector3(-forward.z, 0.0f, forward.x);
        Vector3 up = Vector3.Cross(right, forward);

        for (int i = 0; i < m_NumberOfProjectiles; ++i)
        {
            Ray ray = originalRay;
            float range = m_Range;

            Vector3 maxPosition = originalRay.direction * m_Range;

            //Recoil
            Keyframe[] keys = m_RecoilPattern.keys;
            int keyID = m_CurrentRecoilBullet;
            if (keyID >= keys.Length) keyID = keys.Length - 1;

            maxPosition += up * keys[keyID].time;   //Vertical
            maxPosition += right * keys[keyID].value; //Horizontal

            //Recoil spread
            float spreadRate = m_RecoilSpreadRate.Evaluate(m_CurrentRecoilBullet);
            maxPosition += up * UnityEngine.Random.Range(-spreadRate, spreadRate);
            maxPosition += right * UnityEngine.Random.Range(-spreadRate, spreadRate);

            //Natural gun spread
            if (m_Spread > 0.0f)
            {
                maxPosition += up * UnityEngine.Random.Range(-m_Spread, m_Spread);
                maxPosition += right * UnityEngine.Random.Range(-m_Spread, m_Spread);
            }

            ray.direction = maxPosition.normalized;
            range = maxPosition.magnitude;

            if (m_Piercing) { FirePiercingRay(ray, range); }
            else            { FireRay(ray, range); }
        }

        //Animation & Cooldown
        m_Animator.SetTrigger(m_TriggerName);
        m_ShootCooldownTimer = m_ShootCooldown;

        //Recoil
        m_CurrentRecoilBullet += 1;
        m_RecoilCooldownTimer = m_RecoilCooldownRate.Evaluate(m_CurrentRecoilBullet);

        //Use ammo
        if (AmmoUseEvent != null)
            AmmoUseEvent(m_AmmoUseage);

        return true;
    }

    public override bool StopUse(Ray originalRay)
    {
        //This weapon has no need for this, however it's still generic enough to be included in the interface.
        return false;
    }

    public void FireRay(Ray ray, float range)
    {
        //Fire a single ray (get only the first target)
        RaycastHit hitInfo;
        bool success = Physics.Raycast(ray, out hitInfo, range);

        if (!success)
            return;

        if (m_IgnoredColliders.Contains(hitInfo.collider))
            return;

        GameObject go = hitInfo.collider.gameObject;

        //Did we hit a damageableobject?
        IDamageableObject damageableObject = go.GetComponent<IDamageableObject>();

        if (damageableObject != null)
        {
            //Damage calculation
            int damage = CalculateDamage(ray.origin, hitInfo.point, range, m_Damage);
            damageableObject.Damage(damage);
            return;
        }

        //Did we hit a surface?
        SurfaceType surfaceType = go.GetComponent<SurfaceType>();
        if (surfaceType != null)
        {
            surfaceType.SpawnBulletImpactEffect(hitInfo.point, hitInfo.normal);
        }

        //Did we hit a rigidbody?
        Rigidbody rigidBody = go.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            float impactForce = CalculateImpactForce(ray.origin, hitInfo.point, range, m_ImpactForce);
            rigidBody.AddForceAtPosition(-hitInfo.normal * impactForce, hitInfo.point);
        }
    }

    private void FirePiercingRay(Ray ray, float range)
    {
        //RaycastAll doesn't work properly for convex shapes as it only collides with an object once.

        Ray inverseRay = new Ray(ray.origin + ray.direction * range, ray.direction * -1);

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 5.0f);
        Debug.DrawRay(inverseRay.origin, inverseRay.direction * range, Color.yellow, 5.0f);

        List<RaycastHit> regularList = CustomRaycastAll(ray, range);
        if (regularList.Count == 0)
            return;

        List<RaycastHit> inverseList = CustomRaycastAll(inverseRay, range);

        //Ignore certain colliders (owner)
        if (m_IgnoredColliders != null && m_IgnoredColliders.Count > 0)
        {
            for (int i = regularList.Count - 1; i >= 0; --i)
            {
                if (m_IgnoredColliders.Contains(regularList[i].collider))
                    regularList.RemoveAt(i);
            }

            for (int i = inverseList.Count - 1; i >= 0; --i)
            {
                if (m_IgnoredColliders.Contains(inverseList[i].collider))
                    inverseList.RemoveAt(i);
            }
        }

        //Both parameters get altered when piercing through surfaces
        float currentDamage = m_Damage;
        float currentRange = range;
        List<IDamageableObject> damagedObjects = new List<IDamageableObject>();

        for (int i = 0; i < regularList.Count; ++i)
        {
            RaycastHit hitInfo = regularList[i];

            //If we weren't able to reach this object, we won't hit all the next ones either.
            if (hitInfo.distance >= currentRange)
                return;

            RaycastHit inverseHitInfo = new RaycastHit();

            if (inverseList.Count > regularList.Count - i - 1)
            {
                inverseHitInfo = inverseList[regularList.Count - i - 1];
            }

            GameObject go = hitInfo.collider.gameObject;

            //Did we hit a damageableobject?
            IDamageableObject damageableObject = go.GetComponent<IDamageableObject>();

            if (damageableObject == null || damagedObjects.Contains(damageableObject.GetMainDamageableObject()) == false) //Make sure we don't hit the same "main" object twice
            {
                if (damageableObject != null)
                {
                    //Damage calculation
                    int damage = CalculateDamage(ray.origin, hitInfo.point, range, currentDamage);
                    int reserveDamage = damageableObject.Damage(damage);
                    currentDamage -= (damage - reserveDamage);

                    damagedObjects.Add(damageableObject.GetMainDamageableObject());
                }

                //Did we hit a surface?
                SurfaceType surfaceType = go.GetComponent<SurfaceType>();
                if (surfaceType != null)
                {
                    //Paint decal on the front side
                    surfaceType.SpawnBulletImpactEffect(hitInfo.point, hitInfo.normal);

                    if (inverseHitInfo.collider != null) //Possible if we don't end up shooting all the way trough an object
                    {
                        currentDamage -= surfaceType.PiercingDamageFalloffFlat;
                        currentRange -= surfaceType.PiercingRangeFalloffFlat;

                        if (currentDamage > 0.0f && currentRange >= inverseHitInfo.distance)
                        {
                            //Calculate the distance we shot trough
                            float distance = (inverseHitInfo.point - hitInfo.point).magnitude;
                            currentDamage -= surfaceType.PiercingDamageFalloffPerUnit * distance;
                            currentRange -= surfaceType.PiercingRangeFalloffPerUnit * distance;
                        }

                        //Paint decal on the backside if we reached it
                        if (currentDamage > 0.0f) { surfaceType.SpawnBulletImpactEffect(inverseHitInfo.point, inverseHitInfo.normal); }
                    }
                    else
                    {
                        currentDamage = 0.0f;
                    }
                }

                //Did we hit a rigidbody?
                Rigidbody rigidBody = go.GetComponent<Rigidbody>();
                if (rigidBody != null)
                {
                    float impactForce = CalculateImpactForce(ray.origin, hitInfo.point, range, m_ImpactForce);
                    rigidBody.AddForceAtPosition(-hitInfo.normal * impactForce, hitInfo.point);
                }
            }

            //No more damage remaining, no need to continue
            if (currentDamage <= 0)
                return;
        }
    }
    
    private int CalculateDamage(Vector3 start, Vector3 end, float range, float startingValue)
    {
        float distance = (end - start).magnitude;
        float normDistance = distance / range;
        float damagePercentage = m_DamageFalloff.Evaluate(normDistance);

        return Mathf.CeilToInt(startingValue * damagePercentage);
    }

    private float CalculateImpactForce(Vector3 start, Vector3 end, float range, float startingValue)
    {
        float distance = (end - start).magnitude;
        float normDistance = distance / range;
        float damagePercentage = m_DamageFalloff.Evaluate(normDistance);

        return startingValue * damagePercentage;
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

    private void HandleRecoilCooldown()
    {
        if (m_RecoilCooldownTimer > 0.0f && m_ShootCooldownTimer <= 0.0f)
        {
            m_RecoilCooldownTimer -= Time.deltaTime;

            //Can we go down 1 step?
            if (m_CurrentRecoilBullet > 0 &&
                m_RecoilCooldownTimer <= m_RecoilCooldownRate.Evaluate(m_CurrentRecoilBullet - 1))
            {
                m_CurrentRecoilBullet -= 1;
            }

            //Did we hit rock bottom?
            if (m_RecoilCooldownTimer <= 0.0f)
            {
                m_RecoilCooldownTimer = 0.0f;
                m_CurrentRecoilBullet = 0;
            }
        }
    }

    public override bool CanUse()
    {
        return (m_ShootCooldownTimer == 0.0f);
    }


    public List<RaycastHit> CustomRaycastAll(Ray ray, float range)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        float rangeLeft = range;

        while (rangeLeft > 0.0f)
        {
            //Fire a single ray
            RaycastHit hitInfo;
            bool success = Physics.Raycast(ray, out hitInfo, rangeLeft);

            //If we hit something
            if (success && hitInfo.collider != null)
            {
                hits.Add(hitInfo);

                ray.origin = hitInfo.point + (ray.direction * 0.01f); //Otherwise we keep hitting the same object
                rangeLeft -= hitInfo.distance;
            }

            //If we didn't hit anything, stop the loop
            else
            {
                rangeLeft = 0.0f;
            }
        }

        return hits;
    }
}
