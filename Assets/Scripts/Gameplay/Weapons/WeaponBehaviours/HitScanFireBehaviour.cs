using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitScanFireBehaviour : IFireBehaviour
{
    [Header("Damage")]
    [Space(5)]
    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private AnimationCurve m_DamageFalloff;

    [SerializeField]
    private bool m_Piercing;

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

            if (m_Piercing) { FirePiercingRay(ray, range); }
            else            { FireRay(ray, range); }
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
            int damage = CalculateDamage(ray.origin, hitInfo.point, range, m_Damage);

            damageableObject.Damage(damage);
            return;
        }

        //Did we hit a surface?
        SurfaceType surfaceType = go.GetComponent<SurfaceType>();
        if (surfaceType != null)
        {
            DrawDecal(hitInfo, surfaceType.Decal);
        }
    }

    private void FirePiercingRay(Ray ray, float range)
    {
        //Fire a ray in the normal direction
        RaycastHit[] sourceHitInfo = Physics.RaycastAll(ray, range);

        if (sourceHitInfo.Length == 0)
            return;

        //Sort on distance
        List<RaycastHit> sortedRegularList = sourceHitInfo.OrderBy(o => o.distance).ToList();

        //Fire a ray in the opposite direction (used to calculate the width of hit objects)
        Ray inverseRay = new Ray(ray.origin + ray.direction * range, ray.direction * -1);

        sourceHitInfo = Physics.RaycastAll(inverseRay, range);
        List<RaycastHit> sortedInverseList = sourceHitInfo.OrderByDescending(o => o.distance).ToList();
        sortedInverseList.RemoveAt(0); //This removes the player.

        float currentDamage = m_Damage;

        for(int i = 0; i < sortedRegularList.Count; ++i)
        {
            RaycastHit hitInfo = sortedRegularList[i];
            RaycastHit inverseHitInfo = sortedInverseList[i];

            GameObject go = hitInfo.collider.gameObject;

            //Did we hit a damageableobject?
            IDamageableObject damageableObject = go.GetComponent<IDamageableObject>();

            if (damageableObject != null)
            {
                //Damage calculation
                int damage = CalculateDamage(ray.origin, hitInfo.point, range, currentDamage);

                damageableObject.Damage(damage);
                return;
            }

            //Did we hit a surface?
            SurfaceType surfaceType = go.GetComponent<SurfaceType>();
            if (surfaceType != null)
            {
                //Calculate the distance we shot trough
                float distance = (inverseHitInfo.point - hitInfo.point).magnitude;
                currentDamage -= surfaceType.PiercingDamageFalloff * distance;

                //Paint decal on the front side
                DrawDecal(hitInfo, surfaceType.Decal);

                //Paint decal on the backside if we reached it
                if (currentDamage > 0.0f) { DrawDecal(inverseHitInfo, surfaceType.Decal); }
            }

            //No more damage remaining, no need to continue
            if (currentDamage <= 0)
                return;
        }
    }

    private int CalculateDamage(Vector3 start, Vector3 end, float range, float startDamage)
    {
        float distance = (end - start).magnitude;
        float normDistance = distance / range;
        float damagePercentage = m_DamageFalloff.Evaluate(normDistance);

        Debug.Log("Damage percentage: " + damagePercentage + " = " + (startDamage * damagePercentage));
        return Mathf.RoundToInt(startDamage * damagePercentage);
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

    private void DrawDecal(RaycastHit hitInfo, GameObject decal)
    {
        if (decal == null)
            return;

        Vector3 decalPosition = hitInfo.point + (hitInfo.normal * 0.01f); //Offset the decal a bit from the wall
        Quaternion decalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

        GameObject.Instantiate(decal, decalPosition, decalRotation);
    }
}
