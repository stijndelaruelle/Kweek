using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Normally I won't use inherritance because of code reuse.
//But in this case it really makes it more readable
[RequireComponent (typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class BasicPickup : IPickup
{
    [SerializeField]
    private string m_PickupName;
    public override string PickupName
    {
        get { return m_PickupName; }
    }

    private Rigidbody m_Rigidbody;
    private Collider[] m_Colliders;

    private void Awake()
    {
        //GetComponent because dragging collider components in the inspector is very error prone
        //(all the colliders have the same name and there is no way to check if you got them all)
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Colliders = GetComponents<Collider>();
    }

    public override void Pickup(Player player)
    {
        //Picking up a basic pickup does nothing!
    }

    public override void Drop(Vector3 force, Collider throwerCollider)
    {
        //Ignore all collision for a while with the one who dropped us
        StopAllCoroutines();
        StartCoroutine(IgnoreCollisionRoutine(throwerCollider));

        float randomAngle = UnityEngine.Random.Range(-90.0f, 90.0f);
        transform.rotation *= Quaternion.Euler(0.0f, 0.0f, randomAngle);

        if (m_Rigidbody != null)
            m_Rigidbody.AddForce(force);
    }

    private IEnumerator IgnoreCollisionRoutine(Collider throwerCollider)
    {
        //Start ignoring
        foreach (Collider collider in m_Colliders)
        {
            Physics.IgnoreCollision(collider, throwerCollider, true);
        }

        yield return new WaitForSeconds(1.0f);

        //Stop ignoring
        foreach (Collider collider in m_Colliders)
        {
            Physics.IgnoreCollision(collider, throwerCollider, false);
        }
    }
}
