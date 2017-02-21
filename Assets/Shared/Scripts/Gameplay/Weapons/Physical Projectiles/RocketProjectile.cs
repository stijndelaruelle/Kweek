using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : PhysicalProjectile
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject root = collision.gameObject.transform.root.gameObject;

        //Direct hit
        IDamageableObject damageableObject = root.GetComponent<IDamageableObject>();
        Explode(damageableObject);
    }
}
