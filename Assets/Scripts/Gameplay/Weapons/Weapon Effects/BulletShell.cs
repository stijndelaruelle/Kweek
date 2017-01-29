using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_RigidBody;

    //At first we are coupled to our parent. This to get consistent visuals when the player is on the move.
    //After a short time (when we dissappear from the screen, we decouple ourselves to behave normally when landing)

    //This variable is hardcoded as this really is just a cheap fix and not something that should be different for every bulletshell.
    private float m_DecoupleTimer = 0.25f;

    //Because of a bad initial camera setup (the guns are very small) we scale the shells after decoupling so they don't appear to be very tiny on the floor
    private float m_ScaleAfterDecouple = 3.0f;

    public void Eject(Vector3 force)
    {
        m_RigidBody.AddForce(force);
    }

    private void Update()
    {
        if (m_DecoupleTimer > 0.0f)
        {
            m_DecoupleTimer -= Time.deltaTime;

            if (m_DecoupleTimer < 0.0f)
            {
                transform.parent = null;
                transform.localScale = new Vector3(m_ScaleAfterDecouple, m_ScaleAfterDecouple, m_ScaleAfterDecouple);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Fall sound effect randomizer
    }
}
