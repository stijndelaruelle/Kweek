﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Refactor this class (split it up)
public class Weapon : MonoBehaviour
{
    public delegate void SwitchWeaponCallback();

    [Header("Weapon Behaviours")]
    [Space(5)]
    [SerializeField]
    private IFireBehaviour m_FireBehaviour;

    [SerializeField]
    private IFireBehaviour m_AltFireBehaviour;

    [SerializeField]
    private IAmmoUseBehaviour m_AmmoUseBehaviour;

    [Space(10)]
    [Header("Switch times")]
    [Space(5)]
    [SerializeField]
    private float m_SwitchInTime = 1.0f;

    [SerializeField]
    private float m_SwitchOutTime = 1.0f;
    private bool m_IsSwitching = false;

    [Space(10)]
    [Header("Pickup")]
    [Space(5)]
    [SerializeField]
    private WeaponPickup m_PickupPrefab;

    [SerializeField]
    private float m_ThrowSpeed;

    [Space(10)]
    [Header("Animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;

    //Events
    private WeaponFireDelegate m_WeaponFireEvent;
    public WeaponFireDelegate WeaponFireEvent
    {
        get { return m_WeaponFireEvent; }
        set { m_WeaponFireEvent = value; }
    }

    private UpdateAmmoDelegate m_UpdateAmmoEvent;
    public UpdateAmmoDelegate UpdateAmmoEvent
    {
        get { return m_UpdateAmmoEvent; }
        set { m_UpdateAmmoEvent = value; }
    }

    private void OnDestroy()
    {
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UpdateAmmoEvent -= OnUpdateAmmo;
    }

    public void Setup(AmmoArsenal ammoArsenal)
    {
        if (m_AmmoUseBehaviour != null)
        {
            m_AmmoUseBehaviour.UpdateAmmoEvent += OnUpdateAmmo;
            m_AmmoUseBehaviour.Setup(ammoArsenal);
        }
    }

    //Shooting
    public void Fire()
    {
        if (m_IsSwitching)
            return;

        ExecuteFireBehaviour(m_FireBehaviour);
    }

    public void AltFire()
    {
        if (m_IsSwitching)
            return;

        ExecuteFireBehaviour(m_AltFireBehaviour);
    }

    public void PerformAction()
    {
        m_AmmoUseBehaviour.PerformAction();
    }

    private void ExecuteFireBehaviour(IFireBehaviour fireBehaviour)
    {
        //Check if we can fire
        if (m_FireBehaviour != null && m_FireBehaviour.CanFire() == false)
            return;

        if (m_AltFireBehaviour != null && m_AltFireBehaviour.CanFire() == false)
            return;

        if (m_AmmoUseBehaviour != null && m_AmmoUseBehaviour.CanUse() == false)
            return;

        if (m_IsSwitching)
            return;

        //Fire the weapon
        if (fireBehaviour != null)
            fireBehaviour.Fire();

        //Shooting consequences
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UseAmmo(fireBehaviour.GetAmmoUseage());

        FireWeaponFireEvent();
    }

    //Switching
    public void SwitchOut(SwitchWeaponCallback callback)
    {
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.Cancel();

        m_Animator.SetTrigger("SwitchOutTrigger");
        StartCoroutine(SwitchOutRoutine(callback));
    }

    public void SwitchIn(SwitchWeaponCallback callback)
    {
        m_Animator.SetTrigger("SwitchInTrigger");
        //Play switch sound

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
    }

    //Dropping
    public void Drop(Vector3 position, Collider throwerCollider)
    {
        Ray centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        Vector3 velocity = centerRay.direction * m_ThrowSpeed;

        Quaternion rotation = transform.rotation * m_PickupPrefab.transform.localRotation;
        WeaponPickup pickup = GameObject.Instantiate(m_PickupPrefab, position, rotation);

        if (m_AmmoUseBehaviour != null)
        {
            m_AmmoUseBehaviour.SetPickupAmmo(pickup);
        }

        pickup.Drop(velocity, throwerCollider);
    }

    //Event
    public void FireWeaponFireEvent()
    {
        if (m_WeaponFireEvent != null)
            m_WeaponFireEvent();
    }

    private void OnUpdateAmmo(int ammoInClip, int ammoInReserve)
    {
        //Forward the event
        if (m_UpdateAmmoEvent != null)
            m_UpdateAmmoEvent(ammoInClip, ammoInReserve);
    }
}
