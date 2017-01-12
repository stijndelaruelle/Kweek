﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProjectile : MonoBehaviour
{
    [SerializeField]
    protected int m_DirectImpactDamage;

    [SerializeField]
    protected int m_ExplosionDamage;

    [SerializeField]
    protected float m_ExplosionRadius;

    [SerializeField]
    protected float m_ExplosionForce;

    [SerializeField]
    private float m_LifeTime;
    private float m_Counter;

    [SerializeField]
    private float m_InitialSpeed;

    [SerializeField]
    private Rigidbody m_Rigidbody;

    public void Fire(Vector3 direction, Vector3 baseVelocity)
    {
        Vector3 force = (direction * m_InitialSpeed) + baseVelocity;
        m_Rigidbody.AddForce(force);
    }

    private void Update()
    {
        HandleLifeTime();
    }

    private void HandleLifeTime()
    {
        m_Counter += Time.deltaTime;

        if (m_Counter >= m_LifeTime)
        {
            Explode(null);
        }
    }

    protected void Explode(IDamageableObject directImpactTarget)
    {
        //Explosion damage & pushback
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        List<GameObject> hitObjects = new List<GameObject>();

        //Collect all the unique objects (objects can consist of multiple colliders)
        for (int i = 0; i < colliders.Length; ++i)
        {
            GameObject root = null;

            DamageablePart damageablePart = colliders[i].transform.gameObject.GetComponent<DamageablePart>();
            if (damageablePart != null) root = damageablePart.MainObject;

            if (root != null && !hitObjects.Contains(root))
            {
                hitObjects.Add(root);
            }
        }

        //For each unique object do damage & push back calculations
        foreach (GameObject hitObject in hitObjects)
        {
            IDamageableObject damageableObject = hitObject.GetComponent<IDamageableObject>();
            IMoveableObject moveableObject = hitObject.GetComponent<IMoveableObject>();

            Vector3 centerOfMass = transform.position;
            if (moveableObject != null) centerOfMass = moveableObject.GetCenterOfMass();

            //Get the center position of 
            //Calculate distance for damage falloff & explosion force
            Vector3 direction = (centerOfMass - transform.position);
            Vector3 normDirection = direction.normalized;

            float distance = direction.magnitude;
            float normDistance = distance / m_ExplosionRadius; //0 = super close, 1 = furthest away
            if (normDistance > 1.0f) normDistance = 1.0f;
            float inversedNormDistance = 1.0f - normDistance;  //1 = super close, 0 = furthest away (used as a multiplier)

            if (damageableObject != null && damageableObject != directImpactTarget) //directImpactTarget already had his fair share
            {
                damageableObject.Damage((int)(m_ExplosionDamage * inversedNormDistance));
            }

            if (moveableObject != null)
            {
                //Always fly up a bit (for fun)
                normDirection.y += 0.75f;
                normDirection.Normalize();

                moveableObject.AddVelocity(normDirection * (m_ExplosionForce * inversedNormDistance));
                Debug.Log(inversedNormDistance);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }
}