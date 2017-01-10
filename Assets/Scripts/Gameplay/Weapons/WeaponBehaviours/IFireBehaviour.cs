using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Unity really doesn't like interfaces in the inspector
//public interface IFireBehaviour
//{
//    void Fire();
//}

public abstract class IFireBehaviour : MonoBehaviour
{
    public abstract void Fire();

    public abstract bool CanFire();
    public abstract int GetAmmoUseage();
}
