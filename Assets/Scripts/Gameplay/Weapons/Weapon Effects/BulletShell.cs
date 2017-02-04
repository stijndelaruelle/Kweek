using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_RigidBody;

    [SerializeField]
    private Collider m_Collider;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip[] m_AudioClips;
    private bool m_HasPlayedClip = false;

    [SerializeField]
    private bool m_DelayedDecouple = true; //Only the player uses this (cheap fix)

    //At first we are coupled to our parent. This to get consistent visuals when the player is on the move.
    //After a short time (when we dissappear from the screen, we decouple ourselves to behave normally when landing)

    //This variable is hardcoded as this really is just a cheap fix and not something that should be different for every bulletshell.
    private float m_DecoupleTimer = 0.25f;

    //Because of a bad initial camera setup (the guns are very small) we scale the shells after decoupling so they don't appear to be very tiny on the floor
    private float m_ScaleAfterDecouple = 3.0f;

    public void Eject(Vector3 force, List<Collider> ignoreColliders)
    {
        foreach (Collider otherCollider in ignoreColliders)
        {
            Physics.IgnoreCollision(m_Collider, otherCollider);
        }

        if (m_DelayedDecouple == false)
        {
            Decouple();
        }

        m_RigidBody.AddForce(force);
    }

    private void Update()
    {
        if (m_DelayedDecouple && m_DecoupleTimer > 0.0f)
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
        if (m_AudioClips.Length <= 0 || m_HasPlayedClip == true)
            return;

        int randClip = 0;
        if (m_AudioClips.Length > 1)
            randClip = UnityEngine.Random.Range(0, m_AudioClips.Length - 1);

        m_AudioSource.clip = m_AudioClips[randClip];
        m_AudioSource.Play();

        m_HasPlayedClip = true;
    }
}
