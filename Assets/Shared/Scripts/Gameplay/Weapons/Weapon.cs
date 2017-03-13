using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public delegate void SwitchWeaponCallback();

    [Header("Weapon Behaviours")]
    [Space(5)]
    [SerializeField]
    private IWeaponUseBehaviour m_UseBehaviour;

    [SerializeField]
    private IWeaponUseBehaviour m_AltUseBehaviour;

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
    public float ThrowSpeed
    {
        get { return m_ThrowSpeed; }
    }

    [Space(10)]
    [Header("Switching animation")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;

    private List<Collider> m_OwnerColliders;
    public List<Collider> OwnerCollider
    {
        get { return m_OwnerColliders; }
    }

    //Events
    public event WeaponUseDelegate WeaponUseEvent;
    public event WeaponUseDelegate WeaponStopUseEvent;
    public event UpdateAmmoDelegate UpdateAmmoEvent;

    public event SwitchWeaponCallback StartSwitchInEvent;
    public event SwitchWeaponCallback StopSwitchInEvent;
    public event SwitchWeaponCallback StartSwitchOutEvent;
    public event SwitchWeaponCallback StopSwitchOutEvent;

    private void OnDestroy()
    {
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UpdateAmmoEvent -= OnUpdateAmmo;
    }

    public void Setup(List<Collider> ownerColliders, AmmoArsenal ammoArsenal)
    {
        m_OwnerColliders = ownerColliders;

        if (m_UseBehaviour != null)    { m_UseBehaviour.Setup(ownerColliders); }
        if (m_AltUseBehaviour != null) { m_AltUseBehaviour.Setup(ownerColliders); }

        if (m_AmmoUseBehaviour != null)
        {
            m_AmmoUseBehaviour.UpdateAmmoEvent += OnUpdateAmmo;
            m_AmmoUseBehaviour.Setup(this, ammoArsenal);
        }
    }

    //Using (firing/swinging/starting to charge...)
    public bool Use()
    {
        return Use(new Ray());
    }

    public bool Use(Ray originalRay)
    {
        if (m_IsSwitching)
            return false;

        return ExecuteWeaponUseBehaviour(m_UseBehaviour, originalRay);
    }

    public bool StopUse()
    {
        return StopUse(new Ray());
    }

    public bool StopUse(Ray originalRay)
    {
        if (m_IsSwitching)
            return false;

        return ExecuteWeaponStopUseBehaviour(m_UseBehaviour, originalRay);
    }


    public bool AltUse()
    {
        return AltUse(new Ray());
    }

    public bool AltUse(Ray originalRay)
    {
        if (m_IsSwitching)
            return false;

        return ExecuteWeaponUseBehaviour(m_AltUseBehaviour, originalRay);
    }

    public bool AltStopUse()
    {
        return AltStopUse(new Ray());
    }

    public bool AltStopUse(Ray originalRay)
    {
        if (m_IsSwitching)
            return false;

        return ExecuteWeaponStopUseBehaviour(m_AltUseBehaviour, originalRay);
    }

    //Mostly used for reloading, could become an array (f.e. weapon bashing)
    public void PerformAction()
    {
        m_AmmoUseBehaviour.PerformAction();
    }


    private bool ExecuteWeaponUseBehaviour(IWeaponUseBehaviour fireBehaviour, Ray originalRay)
    {
        //Check if we can fire
        if (m_UseBehaviour != null && m_UseBehaviour.CanUse() == false)
            return false;

        if (m_AltUseBehaviour != null && m_AltUseBehaviour.CanUse() == false)
            return false;

        if (m_AmmoUseBehaviour != null && m_AmmoUseBehaviour.CanUse() == false)
            return false;

        if (m_IsSwitching)
            return false;

        //Use the weapon
        if (fireBehaviour != null)
        {
            bool success = fireBehaviour.Use(originalRay);
            if (success == false)
                return false;
        }

        //Shooting consequences
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UseAmmo(fireBehaviour.GetAmmoUseage());

        FireWeaponUseEvent();
        return true;
    }

    private bool ExecuteWeaponStopUseBehaviour(IWeaponUseBehaviour fireBehaviour, Ray originalRay)
    {
        //Check if we can fire
        if (m_UseBehaviour != null && m_UseBehaviour.CanUse() == false)
            return false;

        if (m_AltUseBehaviour != null && m_AltUseBehaviour.CanUse() == false)
            return false;

        if (m_AmmoUseBehaviour != null && m_AmmoUseBehaviour.CanUse() == false)
            return false;

        if (m_IsSwitching)
            return false;

        //Stop using the weapon
        if (fireBehaviour != null)
        {
            bool success = fireBehaviour.StopUse(originalRay);
            if (success == false)
                return false;
        }

        //Shooting consequences
        if (m_AmmoUseBehaviour != null)
            m_AmmoUseBehaviour.UseAmmo(fireBehaviour.GetAmmoUseage());

        FireWeaponStopUseEvent();
        return true;
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

        if (StartSwitchOutEvent != null)
            StartSwitchOutEvent();

        yield return new WaitForSeconds(m_SwitchOutTime);

        if (StopSwitchOutEvent != null)
            StopSwitchOutEvent();

        callback();
        m_IsSwitching = false;
    }

    private IEnumerator SwitchInRoutine(SwitchWeaponCallback callback)
    {
        m_IsSwitching = true;

        if (StartSwitchInEvent != null)
            StartSwitchInEvent();

        yield return new WaitForSeconds(m_SwitchInTime);

        if (StopSwitchInEvent != null)
            StopSwitchInEvent();

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
    public void FireWeaponUseEvent()
    {
        if (WeaponUseEvent != null)
            WeaponUseEvent();
    }

    public void FireWeaponStopUseEvent()
    {
        if (WeaponStopUseEvent != null)
            WeaponStopUseEvent();
    }

    private void OnUpdateAmmo(int ammoInClip, int ammoInReserve)
    {
        //Forward the event
        if (UpdateAmmoEvent != null)
            UpdateAmmoEvent(ammoInClip, ammoInReserve);
    }
}
