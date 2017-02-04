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

    [SerializeField]
    protected AmmoTypeDefinition m_AmmoType;
    protected AmmoArsenal m_AmmoArsenal;

    [SerializeField]
    protected float m_ReloadTime = 0.0f;
    protected float m_ReloadTimer;

    [Space(10)]
    [Header("Animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;

    [Space(10)]
    [Header("Sound")]
    [Space(5)]
    [SerializeField]
    protected AudioSource m_OutOfAmmoAudio; //Not an event as it's very specific to ammo dependant weapons

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
    }

    private void OnEnable()
    {
        FireUpdateAmmoEvent();
    }

    private void OnDestroy()
    {
        if (m_AmmoArsenal != null)
            m_AmmoArsenal.UpdateReserveAmmoEvent -= OnUpdateReserveAmmo;
    }

    private void Update()
    {
        HandleReloadTimer();
    }

    public override void PerformAction()
    {
        StartReload();
    }

    public override void Setup(AmmoArsenal ammoArsenal)
    {
        m_AmmoArsenal = ammoArsenal;

        if (m_AmmoArsenal != null)
            m_AmmoArsenal.UpdateReserveAmmoEvent += OnUpdateReserveAmmo;
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
        if (m_AmmoArsenal != null && m_AmmoArsenal.GetAmmo(m_AmmoType) <= 0)
            return;

        //We are already reloading or switching
        if (m_ReloadTimer > 0.0f)
            return;

        //Actually start reloading
        if (m_Animator != null)
        {
            m_Animator.SetTrigger("ReloadTrigger");
        }

        m_ReloadTimer = m_ReloadTime;
    }

    protected virtual void EndReload()
    {
        int addedAmmo = m_MaxAmmoInClip - m_AmmoInClip;

        //If we don't have enough ammo, use everything we have
        if (m_AmmoArsenal != null)
        {
            int reserveAmmo = m_AmmoArsenal.GetAmmo(m_AmmoType);
            if (reserveAmmo < addedAmmo)
                addedAmmo = reserveAmmo;
        }

        //Add ammo in clip before removing because of the autoreload call from OnUpdateReserveAmmo.
        m_AmmoInClip += addedAmmo;

        if (m_AmmoArsenal != null)
        {
            m_AmmoArsenal.ChangeAmmo(m_AmmoType, -addedAmmo);
        }

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
        //If the weapon tries to use us, but we're out of ammo.
        if (m_ReloadTimer == 0.0f && m_AmmoInClip <= 0)
        {
            if (m_OutOfAmmoAudio != null && m_OutOfAmmoAudio.isPlaying == false)
                m_OutOfAmmoAudio.Play();
        }

        return (m_ReloadTimer == 0.0f && m_AmmoInClip > 0);
    }

    protected void FireUpdateAmmoEvent()
    {
        if (m_UpdateAmmoEvent != null)
        {
            int reserveAmmo = 0;
            if (m_AmmoArsenal != null) { reserveAmmo = m_AmmoArsenal.GetAmmo(m_AmmoType); }

            m_UpdateAmmoEvent(m_AmmoInClip, reserveAmmo);
        } 
    }

    private void OnUpdateReserveAmmo(AmmoTypeDefinition ammoType, int amount)
    {
        if (m_AmmoType == ammoType)
            FireUpdateAmmoEvent();

        AutoReload();
    }

    public void SetAmmo(int ammoInClip)
    {
        //Mainly used by the ammo pickup, ammo will soon be implemented diffrently

        if (ammoInClip > m_MaxAmmoInClip) { ammoInClip = m_MaxAmmoInClip; }
        if (ammoInClip < 0)               { ammoInClip = 0; }

        m_AmmoInClip = ammoInClip;

        FireUpdateAmmoEvent();
    }

    public override void SetPickupAmmo(WeaponPickup pickup)
    {
        pickup.Ammo = m_AmmoInClip;
    }
}
