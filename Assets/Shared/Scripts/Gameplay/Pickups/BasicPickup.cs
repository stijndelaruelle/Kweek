using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Normally I won't use inherritance because of code reuse.
//But in this case it really makes it more readable
[RequireComponent (typeof(Rigidbody))]
public class BasicPickup : IPickup
{
    [SerializeField]
    private string m_PickupName;
    public override string PickupName
    {
        get { return m_PickupName; }
    }

    [SerializeField]
    private AudioSource m_PickupSoundEffect;

    private Rigidbody m_Rigidbody;
    private List<Collider> m_Colliders;
    private List<Renderer> m_Renderers;

    private GameObject m_UsedImpactEffect = null;

    private void Awake()
    {
        //GetComponent because dragging collider components in the inspector is very error prone
        //(all the colliders have the same name and there is no way to check if you got them all)
        m_Rigidbody = GetComponent<Rigidbody>();

        m_Colliders = new List<Collider>();
        m_Colliders.AddRange(GetComponents<Collider>());
        m_Colliders.AddRange(GetComponentsInChildren<Collider>());

        m_Renderers = new List<Renderer>();
        m_Renderers.AddRange(GetComponents<Renderer>());
        m_Renderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    public override void Pickup(Player player)
    {

    }

    public void OnEnable()
    {
        //Enable the colliders
        foreach (Collider collider in m_Colliders)
        {
            collider.enabled = true;
        }
    }

    public void OnDisable()
    {
        //Disable the colliders
        foreach (Collider collider in m_Colliders)
        {
            collider.enabled = false;
        }
    }

    protected void DestroyPickup()
    {
        StopAllCoroutines();
        StartCoroutine(DestroyPickupRoutine());
    }

    private IEnumerator DestroyPickupRoutine()
    {
        if (m_PickupSoundEffect != null)
            m_PickupSoundEffect.Play();

        //Disable the colliders
        foreach (Collider collider in m_Colliders)
        {
            collider.enabled = false;
        }

        //Disable the renderer
        foreach (Renderer renderer in m_Renderers)
        {
            renderer.enabled = false;
        }

        //Destroy the pickup once the sound effect was played
        yield return new WaitForSeconds(m_PickupSoundEffect.clip.length);

        Destroy(this.gameObject);
    }

    public override void Drop(Vector3 force, List<Collider> throwerColliders)
    {
        //Ignore all collision for a while with the one who dropped us
        if (throwerColliders != null)
        {
            StopAllCoroutines();
            StartCoroutine(IgnoreCollisionRoutine(throwerColliders));
        }

        float randomAngle = UnityEngine.Random.Range(-90.0f, 90.0f);
        transform.rotation *= Quaternion.Euler(0.0f, 0.0f, randomAngle);

        if (m_Rigidbody != null)
            m_Rigidbody.AddForce(force);
    }

    private IEnumerator IgnoreCollisionRoutine(List<Collider> throwerColliders)
    {
        //Todo: Clean up this ignore collision clause.
        //Start ignoring
        if (throwerColliders != null)
        {
            foreach (Collider collider in m_Colliders)
            {
                foreach (Collider otherCollider in throwerColliders)
                {
                    if (otherCollider != null)
                    {
                        Physics.IgnoreCollision(collider, otherCollider, true);
                    } 
                }
            }
        }

        yield return new WaitForSeconds(1.0f);

        //Stop ignoring
        foreach (Collider collider in m_Colliders)
        {
            foreach (Collider otherCollider in throwerColliders)
            {
                if (otherCollider != null)
                {
                    Physics.IgnoreCollision(collider, otherCollider, false);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Play a hit sound
        PlaySurfaceImpactSound(collision);
    }

    private void PlaySurfaceImpactSound(Collision collision)
    {
        if (m_UsedImpactEffect != null)
            return;

        SurfaceType surfaceType = collision.gameObject.GetComponent<SurfaceType>();
        if (surfaceType != null)
        {
            m_UsedImpactEffect = surfaceType.SpawnCharacterImpactEffect(collision.contacts[0].point, collision.contacts[0].normal);
        }
    }

    public void IgnoreColliders(List<Collider> ignoreColliders)
    {
        foreach (Collider collider in m_Colliders)
        {
            foreach (Collider ignoreCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, ignoreCollider, true);
            }
        }
    }
}
