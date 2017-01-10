using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateAmmoDelegate(int i, int j);

public abstract class IAmmoUseBehaviour : MonoBehaviour
{
    public abstract UpdateAmmoDelegate UpdateAmmoEvent
    {
        get;
        set;
    }

    public abstract void UseAmmo(int amount);
    public abstract void Cancel();

    public abstract bool CanUse();
}
