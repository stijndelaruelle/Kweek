using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    public class WeaponArsenal : MonoBehaviour
    {
        [SerializeField]
        private Player m_Player = null;
        private bool m_IsActive = true;

        [SerializeField]
        private int m_MaxWeapons = 2;

        //[SerializeField]
        private int m_CurrentWeaponID = 0;
        private int m_LastWeaponID = -1;

        [SerializeField]
        private List<Weapon> m_Weapons = null;
        private bool m_IsSwitching = false;

        [Header("Required pass trough references")]
        [SerializeField]
        private AmmoArsenal m_AmmoArsenal = null;
        public AmmoArsenal AmmoArsenal
        {
            get { return m_AmmoArsenal; }
        }

        [SerializeField]
        private Transform m_ThrowTransform = null;

        [SerializeField]
        private List<Collider> m_OwnerColliders = null;

        public event UpdateAmmoDelegate UpdateAmmoEvent = null;

        private void Start()
        {
            if (m_Player != null)
            {
                m_Player.DeathEvent += OnPlayerDeath;
                m_Player.RespawnEvent += OnPlayerRespawn;
            }

            //Disable all the weapons
            foreach (Weapon weapon in m_Weapons)
            {
                weapon.gameObject.SetActive(false);
                weapon.Setup(m_OwnerColliders, m_AmmoArsenal);
            }

            //Only enable our current weapon
            SwitchWeapon(m_CurrentWeaponID);
        }

        private void OnDestroy()
        {
            if (m_Player != null)
            {
                m_Player.DeathEvent -= OnPlayerDeath;
                m_Player.RespawnEvent -= OnPlayerRespawn;
            }

            if (m_Weapons.Count > 0)
                m_Weapons[m_CurrentWeaponID].UpdateAmmoEvent -= OnUpdateAmmo;
        }

        private void Update()
        {
            if (!m_IsActive)
                return;

            if (Time.timeScale == 0.0f)
                return;

            //Fire weapons
            if (m_Weapons != null && m_Weapons.Count > 0 && m_CurrentWeaponID >= 0 && m_CurrentWeaponID < m_Weapons.Count)
            {
                Ray originalRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

                if (Input.GetMouseButton(0))
                {
                    m_Weapons[m_CurrentWeaponID].Use(originalRay);
                }
                else
                {
                    m_Weapons[m_CurrentWeaponID].StopUse(originalRay);
                }

                if (Input.GetMouseButton(1))
                {
                    m_Weapons[m_CurrentWeaponID].AltUse(originalRay);
                }
                else
                {
                    m_Weapons[m_CurrentWeaponID].AltStopUse(originalRay);
                }

                if (Input.GetButtonDown("Reload")) { m_Weapons[m_CurrentWeaponID].PerformAction(); } //Input.GetKeyDown(KeyCode.R)
            }

            //Switch to the last used weapon
            if (Input.GetButtonDown("LastUsed")) { SwitchWeapon(m_LastWeaponID); } //Input.GetKeyDown(KeyCode.A)

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

            if (Input.GetButtonDown("Drop")) //Input.GetKeyDown(KeyCode.G))
            {
                //Drop the current weapon
                DropWeapon();

                //Pickup the next one if we have one
                m_LastWeaponID = -1;
                m_CurrentWeaponID++;

                if (m_CurrentWeaponID >= m_Weapons.Count)
                {
                    m_CurrentWeaponID = 0;
                }

                if (m_Weapons.Count > 0)
                {
                    m_IsSwitching = true;
                    OnWeaponSwitchedOut();
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
            if (UpdateAmmoEvent != null)
                UpdateAmmoEvent(ammoInClip, ammoInReserve);
        }

        public Weapon AddWeapon(Weapon weaponPrefab)
        {
            //Todo pool this, make "m_MaxWeapons" of each weapon at the start!
            Weapon instancedWeapon = GameObject.Instantiate(weaponPrefab, transform);

            instancedWeapon.Setup(m_OwnerColliders, m_AmmoArsenal);

            //Even tough the prefabs are at 0, this is still required to actually make it so
            instancedWeapon.transform.localPosition = Vector3.zero;
            instancedWeapon.transform.localRotation = Quaternion.identity;
            instancedWeapon.gameObject.SetActive(false);

            //When we pickup a lot of guns sequentally (while getting ready) it made all the guns inactive
            if (m_IsSwitching == true)
            {
                m_IsSwitching = false;
            }

            if (m_Weapons.Count >= m_MaxWeapons)
            {
                DropWeapon();
                m_Weapons.Insert(m_CurrentWeaponID, instancedWeapon);
                SwitchWeapon(m_CurrentWeaponID);
            }
            else
            {
                m_Weapons.Add(instancedWeapon);
                SwitchWeapon(m_Weapons.Count - 1);
            }

            return instancedWeapon;
        }

        private void RemoveWeapon(Weapon weapon)
        {
            OnUpdateAmmo(0, 0);

            weapon.UpdateAmmoEvent -= OnUpdateAmmo;
            weapon.gameObject.SetActive(false);

            m_Weapons.Remove(weapon);
            GameObject.Destroy(weapon.gameObject);
        }

        private void DropWeapon()
        {
            if (m_CurrentWeaponID == -1 || m_Weapons.Count <= 0)
                return;

            m_Weapons[m_CurrentWeaponID].Drop(m_ThrowTransform.position, m_OwnerColliders);
            RemoveWeapon(m_Weapons[m_CurrentWeaponID]);
        }

        private void OnPlayerDeath()
        {
            m_IsActive = false;
            DropWeapon();
        }

        private void OnPlayerRespawn()
        {
            m_IsActive = true;
        }
    }
}