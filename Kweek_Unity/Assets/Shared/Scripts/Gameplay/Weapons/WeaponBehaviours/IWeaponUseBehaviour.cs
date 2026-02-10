using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    public delegate void WeaponUseDelegate(Vector3 direction);
    public delegate void WeaponStopUseDelegate();
    public delegate void AmmoUseDelegate(int ammo);

    //Unity really doesn't like interfaces in the inspector
    public abstract class IWeaponUseBehaviour : MonoBehaviour
    {
        public abstract event AmmoUseDelegate AmmoUseEvent;

        public abstract void Setup(List<Collider> ownerCollider);

        public abstract bool Use(Ray originalRay);
        public abstract bool StopUse(Ray originalRay);

        public abstract bool CanUse();
    }
}