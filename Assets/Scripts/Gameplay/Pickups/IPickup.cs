using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ChangePickupDelegate(IPickup pickup);

public interface IPickup
{
    string PickupName
    {
        get;
    }

    void Pickup(Player player);
}
