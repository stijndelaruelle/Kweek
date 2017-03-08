using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private IDamageableObject m_DamageableObject;

    [SerializeField]
    private IAIBehaviour m_AIBehaviour;

    [SerializeField]
    private Ragdoll m_Ragdoll;

    private void Start()
    {
        if (m_DamageableObject != null)
        {
            m_DamageableObject.DamageEvent += OnDamage;
            m_DamageableObject.DeathEvent += OnDeath;
        }

        //Enable kinematic (otherwise raycasts will occasionally miss!)
        if (m_Ragdoll != null)
        {
            m_Ragdoll.SetKinematic(true);
            m_Ragdoll.SetActive(false);
        }

        if (m_AIBehaviour != null)
        {
            List<Collider> colliders = new List<Collider>();
            colliders.AddRange(GetComponents<Collider>());
            colliders.AddRange(GetComponentsInChildren<Collider>());

            m_AIBehaviour.Setup(colliders);
        }
    }

    private void OnDestroy()
    {
        if (m_DamageableObject == null)
            return;

        m_DamageableObject.DamageEvent -= OnDamage;
        m_DamageableObject.DeathEvent -= OnDeath;
    }


    //Damage handling
    private void OnDamage(int removedHealth)
    {
        if (m_DamageableObject.Health > 0)
        {
            Debug.Log("THE enemy has " + m_DamageableObject.Health + " left");
            //m_Animator.SetTrigger("WoundTrigger");
            //m_AIBehaviour.Pause();
        }
    }

    private void OnDeath()
    {
        Debug.Log("THE enemy DIED!");

        if (m_Ragdoll != null)
        {
            m_Ragdoll.SetKinematic(false);
            m_Ragdoll.SetActive(true);
        }

        if (m_AIBehaviour != null)
        {
            //m_Animator.SetTrigger("DeathTrigger");
            m_AIBehaviour.OnDeath();
            m_AIBehaviour.enabled = false;
        }
    }
}
