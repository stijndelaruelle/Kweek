using UnityEngine;

namespace Kweek
{
    public class WeaponPickup : BasicPickup
    {
        [Header("Weapon")]
        [SerializeField]
        private Weapon m_FirstPersonWeapon = null;
        protected Weapon m_FirstPersonWeaponInstance = null;

        //Every gun has the ability to have ammo (IAmmoUseBehaviour)
        //How they use this value is completely up to them (see FullClipWeaponPickup for an example)
        [SerializeField]
        protected int m_Ammo = 0;
        public int Ammo
        {
            get { return m_Ammo; }
            set { m_Ammo = value; }
        }

        public override void Pickup(Player player)
        {
            base.Pickup(player);

            WeaponArsenal weaponArsenal = player.WeaponArsenal;
            m_FirstPersonWeaponInstance = weaponArsenal.AddWeapon(m_FirstPersonWeapon);

            DestroyPickup();
        }
    }
}