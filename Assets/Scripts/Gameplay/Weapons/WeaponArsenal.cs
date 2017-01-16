using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArsenal : MonoBehaviour
{
    [SerializeField]
    private int m_CurrentWeaponID = 0;
    private int m_LastWeaponID = -1;

    [SerializeField]
    private List<Weapon> m_Weapons;
    private bool m_IsSwitching = false;

    private UpdateAmmoDelegate m_UpdateAmmoEvent;
    public UpdateAmmoDelegate UpdateAmmoEvent
    {
        get { return m_UpdateAmmoEvent; }
        set { m_UpdateAmmoEvent = value; }
    }

    private void Start()
    {
        //Disable all the weapons
        foreach(Weapon weapon in m_Weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        //Only enable our current weapon
        SwitchWeapon(m_CurrentWeaponID);
    }

    private void OnDestroy()
    {
        m_Weapons[m_CurrentWeaponID].UpdateAmmoEvent -= OnUpdateAmmo;
    }

    private void Update()
    {
        //Switch to the last used weapon
        if (Input.GetKeyDown(KeyCode.A)) { SwitchWeapon(m_LastWeaponID); }

        //Switch weapons with the number keys
        if (Input.GetKeyDown(KeyCode.Keypad1)) { SwitchWeapon(0); }
        if (Input.GetKeyDown(KeyCode.Keypad2)) { SwitchWeapon(1); }
        if (Input.GetKeyDown(KeyCode.Keypad3)) { SwitchWeapon(2); }
        if (Input.GetKeyDown(KeyCode.Keypad4)) { SwitchWeapon(3); }
        if (Input.GetKeyDown(KeyCode.Keypad5)) { SwitchWeapon(4); }
        if (Input.GetKeyDown(KeyCode.Keypad6)) { SwitchWeapon(5); }
        if (Input.GetKeyDown(KeyCode.Keypad7)) { SwitchWeapon(6); }
        if (Input.GetKeyDown(KeyCode.Keypad8)) { SwitchWeapon(7); }
        if (Input.GetKeyDown(KeyCode.Keypad9)) { SwitchWeapon(8); }

        //Switch weapons with the mouse wheel
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel != 0.0f)
        {
            //Previous weapon in the list
            if (mouseWheel > 0.0f)
            {
                int newWeaponID = m_CurrentWeaponID - 1;
                if (newWeaponID < 0) { newWeaponID = m_Weapons.Count - 1; }

                SwitchWeapon(newWeaponID);
            }

            //Next weapon in the list
            else
            {
                int newWeaponID = m_CurrentWeaponID + 1;
                if (newWeaponID >= m_Weapons.Count) { newWeaponID = 0; }

                SwitchWeapon(newWeaponID);
            }
        }
    }

    private void SwitchWeapon(int weaponID)
    {
        //Check if the weapon ID exists
        if (weaponID < 0 || weaponID >= m_Weapons.Count)
            return;

        //Check if we are already switching
        if (m_IsSwitching == true)
            return;

        m_IsSwitching = true;

        //If it's the same weapon as the current one
        if (weaponID == m_CurrentWeaponID)
        {
            OnWeaponSwitchedOut();
            return;
        }

        m_LastWeaponID = m_CurrentWeaponID;
        m_CurrentWeaponID = weaponID;

        //Switching animation
        m_Weapons[m_LastWeaponID].SwitchOut(OnWeaponSwitchedOut);
    }

    private void OnWeaponSwitchedOut()
    {
        if (m_LastWeaponID > -1)
        {
            m_Weapons[m_LastWeaponID].UpdateAmmoEvent -= OnUpdateAmmo;
            m_Weapons[m_LastWeaponID].gameObject.SetActive(false);
        }

        m_Weapons[m_CurrentWeaponID].UpdateAmmoEvent += OnUpdateAmmo; //Subscribe to the ammo event BEFORE SetActive! (OnEnable fires one)
        m_Weapons[m_CurrentWeaponID].gameObject.SetActive(true);


        //Switching animation
        m_Weapons[m_CurrentWeaponID].SwitchIn(OnWeaponSwitchedIn);
    }

    private void OnWeaponSwitchedIn()
    {
        m_IsSwitching = false;
    }

    public void OnUpdateAmmo(int ammoInClip, int ammoInReserve)
    {
        //Forward the event
        if (m_UpdateAmmoEvent != null)
            m_UpdateAmmoEvent(ammoInClip, ammoInReserve);
    }

    public Weapon AddWeapon(Weapon weaponPrefab)
    {
        Weapon instancedWeapon = GameObject.Instantiate(weaponPrefab, transform);

        //Even tough the prefabs are at 0, this is still required to actually make it so
        instancedWeapon.transform.localPosition = Vector3.zero;
        instancedWeapon.transform.localRotation = Quaternion.identity;
        instancedWeapon.gameObject.SetActive(false);

        m_Weapons.Add(instancedWeapon);
        SwitchWeapon(m_Weapons.Count - 1);

        return instancedWeapon;
    }

    private void DropWeapon(Weapon weaponPrefab)
    {
        //Spawn a pickup prefab
    }
}
