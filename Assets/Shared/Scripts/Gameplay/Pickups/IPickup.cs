using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ChangePickupDelegate(IPickup pickup);

public abstract class IPickup : MonoBehaviour
{
    public abstract string PickupName
    {
        get;
    }

    public abstract void Pickup(Player player);
    public abstract void Drop(Vector3 force, List<Collider> throwerColliders);
}
