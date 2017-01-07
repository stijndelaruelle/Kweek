using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableObject
{
    void AddVelocity(Vector3 velocity);
    Vector3 GetCenterOfMass();
}
