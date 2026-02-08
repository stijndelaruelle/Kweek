using UnityEngine;

namespace Kweek
{
    public class AmmoPickup : BasicPickup
    {
        [Header("Ammo")]
        [SerializeField]
        private AmmoTypeDefinition m_AmmoType = null;

        [SerializeField]
        private int m_Amount = 0;

        public override void Pickup(Player player)
        {
            base.Pickup(player);

            AmmoArsenal ammoArsenal = player.WeaponArsenal.AmmoArsenal;

            if (ammoArsenal != null)
            {
                int addedAmmo = m_Amount;

                int diff = m_AmmoType.MaxAmmo - ammoArsenal.GetAmmo(m_AmmoType);
                if (diff < addedAmmo)
                    addedAmmo = diff;

                //If we wouldn't pick up any ammo, don't do anything.
                if (addedAmmo > 0)
                {
                    ammoArsenal.ChangeAmmo(m_AmmoType, m_Amount);
                    DestroyPickup();
                }
            }
        }
    }
}