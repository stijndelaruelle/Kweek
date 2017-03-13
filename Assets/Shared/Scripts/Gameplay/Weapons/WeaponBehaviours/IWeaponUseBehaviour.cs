using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Unity really doesn't like interfaces in the inspector
//public interface IFireBehaviour
//{
//    void Fire();
//}

public delegate void WeaponUseDelegate();

public abstract class IWeaponUseBehaviour : MonoBehaviour
{
    public abstract void Setup(List<Collider> ownerCollider);

    public abstract bool Use(Ray originalRay);
    public abstract bool StopUse(Ray originalRay);

    public abstract bool CanUse();
    public abstract int GetAmmoUseage();
}
