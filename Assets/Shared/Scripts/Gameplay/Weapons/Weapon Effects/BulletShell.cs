using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : PoolableObject
{
    [SerializeField]
    private BulletShellDefinition m_Definition;

    [SerializeField]
    private Rigidbody m_RigidBody;

    [SerializeField]
    private CapsuleCollider m_Collider;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private MeshFilter m_MeshFilter;

    [SerializeField]
    private MeshRenderer m_MeshRenderer;

    private bool m_HasPlayedClip = false;

    //At first we are coupled to our parent. This to get consistent visuals when the player is on the move.
    //After a short time (when we dissappear from the screen, we decouple ourselves to behave normally when landing)

    //This variable is hardcoded as this really is just a cheap fix and not something that should be different for every bulletshell.
    private float m_DecoupleTime = 0.25f;
    private float m_DecoupleTimer = 0.0f;

    //Because of a bad initial camera setup (the guns are very small) we scale the shells after decoupling so they don't appear to be very tiny on the floor
    private float m_ScaleAfterDecouple = 3.0f;

    public void InitializeBulletShell(BulletShellDefinition definition)
    {
        m_Definition = definition;
    }

    public void Eject(Vector3 force, List<Collider> ignoreColliders)
    {
        foreach (Collider otherCollider in ignoreColliders)
        {
            Physics.IgnoreCollision(m_Collider, otherCollider);
        }

        if (m_Definition.DelayedDecouple == false)
        {
            Decouple();
        }

        m_RigidBody.AddForce(force);
    }

    private void Update()
    {
        if (m_Definition == null)
            return;

        if (m_Definition.DelayedDecouple && m_DecoupleTimer > 0.0f)
        {
            m_DecoupleTimer -= Time.deltaTime;

            if (m_DecoupleTimer < 0.0f)
            {
                Decouple();
            }
        }
    }

    private void Decouple()
    {
        transform.parent = null;
        transform.localScale = new Vector3(m_ScaleAfterDecouple, m_ScaleAfterDecouple, m_ScaleAfterDecouple);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_Definition.AudioClips.Count <= 0 || m_HasPlayedClip == true)
            return;

        int randClip = 0;
        if (m_Definition.AudioClips.Count > 1)
            randClip = UnityEngine.Random.Range(0, m_Definition.AudioClips.Count - 1);

        m_AudioSource.clip = m_Definition.AudioClips[randClip];
        m_AudioSource.Play();

        m_HasPlayedClip = true;
    }

    //PoolableObject
    public override void Initialize()
    {
        m_RigidBody.velocity = Vector3.zero;
        m_RigidBody.isKinematic = true;
        m_MeshRenderer.enabled = false;
    }

    public override void Activate()
    {
        //Reset all variables
        m_HasPlayedClip = false;
        m_DecoupleTimer = m_DecoupleTime;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        //Collider
        m_Collider.radius = m_Definition.Radius;
        m_Collider.height = m_Definition.Height;
        m_RigidBody.isKinematic = false;
        m_RigidBody.velocity = Vector3.zero;

        //Mesh
        m_MeshFilter.gameObject.transform.localScale = m_Definition.MeshScale;
        m_MeshFilter.mesh = m_Definition.Mesh;
        m_MeshRenderer.material = m_Definition.Material;
        m_MeshRenderer.enabled = true;
    }

    public override void Deactivate()
    {

    }

    public override bool IsAvailable()
    {
        return true;
    }
}
