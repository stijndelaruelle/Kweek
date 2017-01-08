using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArsenal : MonoBehaviour
{
    [SerializeField]
    private int m_CurrentWeaponID;
    private int m_LastWeaponID;

    [SerializeField]
    private List<Weapon> m_Weapons;
    private bool m_IsSwitching = false;

    private void Start()
    {
        //Disable all the weapons
        foreach(Weapon weapon in m_Weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        //Only enable our current weapon
        m_Weapons[m_CurrentWeaponID].gameObject.SetActive(true);
        m_LastWeaponID = m_CurrentWeaponID;
    }

    private void Update()
    {
        //Actions
        if (Input.GetMouseButton(0))     { Fire(); }
        if (Input.GetKeyDown(KeyCode.R)) { StartReload(); }

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

    public void Fire()
    {
        m_Weapons[m_CurrentWeaponID].Fire();
    }

    public void StartReload()
    {
        m_Weapons[m_CurrentWeaponID].StartReload();
    }

    private void SwitchWeapon(int weaponID)
    {
        //Check if the weapon ID exists
        if (weaponID < 0 || weaponID >= m_Weapons.Count)
            return;

        //Check if we are already switching
        if (m_IsSwitching == true)
            return;

        m_LastWeaponID = m_CurrentWeaponID;
        m_CurrentWeaponID = weaponID;

        //Switching animation
        m_Weapons[m_LastWeaponID].SwitchOut(OnWeaponSwitchedOut);

        m_IsSwitching = true;
    }

    private void OnWeaponSwitchedOut()
    {
        m_Weapons[m_LastWeaponID].gameObject.SetActive(false);
        m_Weapons[m_CurrentWeaponID].gameObject.SetActive(true);

        //Switching animation
        m_Weapons[m_CurrentWeaponID].SwitchIn(OnWeaponSwitchedIn);
    }

    private void OnWeaponSwitchedIn()
    {
        m_IsSwitching = false;
    }

    public void AddWeapon(Weapon weapon)
    {
        m_Weapons.Add(weapon);
    }
}
