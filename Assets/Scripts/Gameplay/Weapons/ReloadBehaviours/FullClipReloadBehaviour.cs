using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullClipReloadBehaviour : IAmmoUseBehaviour
{
    [Space(10)]
    [Header("Ammo options")]
    [Space(5)]
    [SerializeField]
    protected int m_MaxAmmoInClip;
    protected int m_AmmoInClip;

    //Will be moved to another class so guns can share ammo types
    [SerializeField]
    protected int m_MaxReserveAmmo;
    protected int m_ReserveAmmo;

    [SerializeField]
    protected float m_ReloadTime = 0.0f;
    protected float m_ReloadTimer;

    //[SerializeField]
    //private bool m_CanCancel = false;

    [Space(10)]
    [Header("Animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;

    //Event
    private UpdateAmmoDelegate m_UpdateAmmoEvent;
    public override UpdateAmmoDelegate UpdateAmmoEvent
    {
        get { return m_UpdateAmmoEvent; }
        set { m_UpdateAmmoEvent = value; }
    }

    private void Awake()
    {
        m_AmmoInClip = m_MaxAmmoInClip;
        m_ReserveAmmo = m_MaxReserveAmmo;
    }

    private void OnEnable()
    {
        FireUpdateAmmoEvent();
    }

    private void Update()
    {
        //In here and not in the weapon, as not every weapon needs reloading
        if (Input.GetKeyDown(KeyCode.R)) { StartReload(); }

        HandleReloadTimer();
    }

    public override void UseAmmo(int amount)
    {
        CancelReload();

        if (m_AmmoInClip >= amount)
        {
            m_AmmoInClip -= amount;
            FireUpdateAmmoEvent();
        }

        //Check if we need to reload
        AutoReload();
    }

    public override void Cancel()
    {
        CancelReload();
    }


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

        //If we don't have any more ammo
        if (m_ReserveAmmo == 0)
            return;

        //We are already reloading or switching
        if (m_ReloadTimer > 0.0f)
            return;

        //Actually start reloading
        m_Animator.SetTrigger("ReloadTrigger");
        m_ReloadTimer = m_ReloadTime;
    }

    protected virtual void EndReload()
    {
        int addedAmmo = m_MaxAmmoInClip - m_AmmoInClip;

        //If we don't have enough ammo, use everything we have
        if (m_ReserveAmmo < addedAmmo)
            addedAmmo = m_ReserveAmmo;

        m_ReserveAmmo -= addedAmmo;
        m_AmmoInClip += addedAmmo;

        FireUpdateAmmoEvent();

        m_ReloadTimer = 0.0f;
    }

    private void CancelReload()
    {
        m_ReloadTimer = 0.0f;
    }

    private void HandleReloadTimer()
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

    protected void AutoReload()
    {
        if (m_AmmoInClip <= 0)
        {
            StartReload();
        }
    }


    public override bool CanUse()
    {
        return (m_ReloadTimer == 0.0f && m_AmmoInClip > 0); // || m_CanCancel
    }

    protected void FireUpdateAmmoEvent()
    {
        if (m_UpdateAmmoEvent != null)
            m_UpdateAmmoEvent(m_AmmoInClip, m_ReserveAmmo);
    }
}
