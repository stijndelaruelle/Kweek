using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Refactor this class (split it up)
public class Weapon : MonoBehaviour
{
    public delegate void SwitchWeaponCallback();

    [Header("Projectile options")]
    [Space(5)]
    [SerializeField]
    private int m_NumberOfProjectiles;

    [Tooltip("Max spread at max range.")]
    [SerializeField]
    private float m_Spread;

    [Tooltip("Max distance that the projectile will fly.")]
    [SerializeField]
    private float m_Range;

    [SerializeField]
    private float m_ShootCooldown = 1.0f;
    private float m_ShootCooldownTimer = 0.0f;

    [SerializeField]
    private Transform m_ProjectileSpawn;

    [SerializeField]
    private GameObject m_Projectile;

    [Space(10)]
    [Header("Ammo options")]
    [Space(5)]
    [SerializeField]
    private float m_ReloadTime = 1.0f;
    private float m_ReloadTimer = 0.0f;

    [SerializeField]
    private int m_MaxAmmoInClip;
    private int m_AmmoInClip;

    //Will be moved to another class so guns can share ammo types
    [SerializeField]
    private int m_MaxReserveAmmo;
    private int m_ReserveAmmo;

    [Space(10)]
    [Header("Switch times")]
    [Space(5)]
    [SerializeField]
    private float m_SwitchInTime = 1.0f;

    [SerializeField]
    private float m_SwitchOutTime = 1.0f;
    private bool m_IsSwitching = false;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private PlayerController m_PlayerController;

    private void Awake()
    {
        m_AmmoInClip = m_MaxAmmoInClip;
        m_ReserveAmmo = m_MaxReserveAmmo;
    }

    private void Update()
    {
        if (m_IsSwitching)
            return;

        //Cooldowns
        HandleShootingCooldown();
        HandleReloading();
    }

    //Shooting
    public void Fire()
    {
        //Weapon is cooling down, reloading or switching out/in
        if (m_ShootCooldownTimer > 0.0f || m_ReloadTimer > 0.0f || m_IsSwitching == true)
            return;

        //We don't have any more ammo
        if (AutoReload())
            return;

        //All the calculations
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

                maxPosition += right * Random.Range(-m_Spread, m_Spread);
                maxPosition += up * Random.Range(-m_Spread, m_Spread);

                ray.direction = maxPosition.normalized;
                range = maxPosition.magnitude;
            }

            //Can't instantiate interfaces. Preferred this setup above an abstract class. (it's overall more consistent, but a little bit messier here)
            GameObject go = GameObject.Instantiate<GameObject>(m_Projectile, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation);
            IProjectile projectile = go.GetComponent<IProjectile>();

            if (projectile != null)
            {
                projectile.Fire(m_PlayerController.CurrentVelocity, ray, range);
            }
        }

        //Trigger the animation
        m_Animator.SetTrigger("FireTrigger");

        //Shooting consequences
        m_ShootCooldownTimer = m_ShootCooldown;
        m_AmmoInClip -= 1;

        //Auto reload
        AutoReload();
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

    //Reloading
    public void StartReload()
    {
        //We don't reload (Doom/Quake style)
        if (m_ReloadTime <= 0.0f)
        {
            EndReload();
            return;
        }

        //We don't need to reload
        if (m_AmmoInClip == m_MaxAmmoInClip)
            return;

        //If we don't have any more ammo, don't reload
        if (m_ReserveAmmo == 0)
            return;

        //We are already reloading or switching
        if (m_ReloadTimer > 0.0f || m_IsSwitching == true)
            return;

        //Actually start reloading
        m_Animator.SetTrigger("ReloadTrigger");
        m_ReloadTimer = m_ReloadTime;
    }

    private void EndReload()
    {
        int addedAmmo = m_MaxAmmoInClip - m_AmmoInClip;

        //If we don't have enough ammo, use everything we have
        if (m_ReserveAmmo < addedAmmo)
            addedAmmo = m_ReserveAmmo;

        m_ReserveAmmo -= addedAmmo;
        m_AmmoInClip += addedAmmo;

        m_ReloadTimer = 0.0f;
    }

    private void CancelReload()
    {
        m_ReloadTimer = 0.0f;
    }

    private void HandleReloading()
    {
        if (m_ReloadTimer > 0.0f)
        {
            m_ReloadTimer -= Time.deltaTime;

            if (m_ReloadTimer <= 0.0f)
            {
                EndReload();
            }
        }
    }

    private bool AutoReload()
    {
        if (m_AmmoInClip <= 0)
        {
            StartReload();
            return true;
        }

        return false;
    }

    //Switching
    public void SwitchOut(SwitchWeaponCallback callback)
    {
        CancelReload();

        m_Animator.SetTrigger("SwitchOutTrigger");
        StartCoroutine(SwitchOutRoutine(callback));
    }

    public void SwitchIn(SwitchWeaponCallback callback)
    {
        m_Animator.SetTrigger("SwitchInTrigger");
        StartCoroutine(SwitchInRoutine(callback));
    }

    private IEnumerator SwitchOutRoutine(SwitchWeaponCallback callback)
    {
        m_IsSwitching = true;

        yield return new WaitForSeconds(m_SwitchOutTime);

        callback();
        m_IsSwitching = false;
    }

    private IEnumerator SwitchInRoutine(SwitchWeaponCallback callback)
    {
        m_IsSwitching = true;

        yield return new WaitForSeconds(m_SwitchInTime);

        callback();
        m_IsSwitching = false;

        //Check if this gun needs reloading
        AutoReload();
    }
}
