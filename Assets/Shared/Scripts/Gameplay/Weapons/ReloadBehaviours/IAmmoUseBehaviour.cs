using UnityEngine;

namespace Kweek
{
    public delegate void UpdateAmmoDelegate(int ammoInClip, int ammoInReserve);

    public abstract class IAmmoUseBehaviour : MonoBehaviour
    {
        public event UpdateAmmoDelegate UpdateAmmoEvent = null;
        protected void FireUpdateAmmoEvent(int ammoInClip, int ammoInReserve)
        {
            if (UpdateAmmoEvent != null)
                UpdateAmmoEvent(ammoInClip, ammoInReserve);
        }

        public abstract void Setup(Weapon weapon, AmmoArsenal ammoArsenal);
        public abstract void PerformAction();

        public abstract void UseAmmo(int amount);
        public abstract void Cancel();

        public abstract bool CanUse();

        public abstract void SetPickupAmmo(WeaponPickup pickup);
    }
}