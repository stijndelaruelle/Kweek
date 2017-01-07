using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void Fire(Vector3 baseVelocity, Ray ray, float range);
}
