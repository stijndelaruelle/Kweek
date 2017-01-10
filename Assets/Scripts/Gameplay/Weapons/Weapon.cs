using System.Collections;
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
    [Header("Animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;

    //Events
    private UpdateAmmoDelegate m_UpdateAmmoEvent;
    public UpdateAmmoDelegate UpdateAmmoEvent
    {
        get { return m_UpdateAmmoEvent; }
        set { m_UpdateAmmoEvent = value; }
    }

    private void Awake()
    {
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UpdateAmmoEvent += OnUpdateAmmo;
    }

    private void OnDestroy()
    {
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UpdateAmmoEvent -= OnUpdateAmmo;
    }

    private void Update()
    {
        if (m_IsSwitching)
            return;

        if (Input.GetMouseButton(0))     { Fire(); }
        if (Input.GetMouseButton(1))     { AltFire(); }
    }

    //Shooting
    public void Fire()
    {
        ExecuteFireBehaviour(m_FireBehaviour);
    }

    public void AltFire()
    {
        ExecuteFireBehaviour(m_AltFireBehaviour);
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

        //FireUpdateAmmoEvent();
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

    //Event
    public void OnUpdateAmmo(int ammoInClip, int ammoInReserve)
    {
        //Forward the event
        if (m_UpdateAmmoEvent != null)
            m_UpdateAmmoEvent(ammoInClip, ammoInReserve);
    }
}
