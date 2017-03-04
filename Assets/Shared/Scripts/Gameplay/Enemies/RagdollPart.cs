using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Once collisions are enabled this script turns the character in a ragdoll once it hits a surface
//F.e.: The animator will do an animation until they hit a wall, then the ragdoll will take over

public class RagdollPart : MonoBehaviour
{
    [SerializeField]
    private Ragdoll m_Ragdoll;
    private List<GameObject> m_OtherParts;

    private void Start()
    {
        m_OtherParts = new List<GameObject>();

        if (m_Ragdoll != null)
        {
            foreach (RagdollPart part in m_Ragdoll.RagdollParts)
            {
                m_OtherParts.Add(part.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_Ragdoll != null)
        {
            if (m_OtherParts.Contains(collision.gameObject))
                return;

            //Play a hit sound
            PlaySurfaceImpactSound(collision);

            if (m_Ragdoll.IsRagdollEnabled())
                return;

            //Don't start ragdolls when we just hit a regular floor (not a slope!)
            if (collision.contacts[0].normal == Vector3.up)
                return;

            Debug.Log(gameObject.name + " enabled the ragdoll!", gameObject);
            m_Ragdoll.SetActive(true);
        }
    }

    private void PlaySurfaceImpactSound(Collision collision)
    {
        SurfaceType surfaceType = collision.gameObject.GetComponent<SurfaceType>();
        if (surfaceType != null)
        {
            surfaceType.SpawnImpactEffect(collision.contacts[0].point);
        }
    }
}
