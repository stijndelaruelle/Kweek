using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IMoveableObject
{
    [SerializeField]
    private IDamageableObject m_DamageableObject;

    //[SerializeField]
    //private IAIBehaviour m_AIBehaviour; //The time will come...
    [SerializeField]
    private PatrolBehaviour m_AIBehaviour;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Rigidbody m_MainRigidbody;
    public Rigidbody MainRigidbody
    {
        get { return m_MainRigidbody; }
    }

    [SerializeField]
    private List<GameObject> m_RagdollParts;
    public List<GameObject> RagdollParts
    {
        get { return m_RagdollParts; }
    }

    private Rigidbody[] m_Rigidbodies;

    private void Awake()
    {
        //Enable kinematic (otherwise raycasts will occasionally miss!)
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].isKinematic = true;
        }
    }

    private void Start()
    {
        m_DamageableObject.DamageEvent += OnDamage;
        m_DamageableObject.DeathEvent += OnDeath;
    }

    private void OnDestroy()
    {
        if (m_DamageableObject == null)
            return;

        m_DamageableObject.DamageEvent -= OnDamage;
        m_DamageableObject.DeathEvent -= OnDeath;
    }

    //Damage handling
    private void OnDamage()
    {
        if (m_DamageableObject.Health > 0)
        {
            Debug.Log("THE enemy has " + m_DamageableObject.Health + " left");
            m_Animator.SetTrigger("WoundTrigger");
            m_AIBehaviour.Pause();
        }
    }

    private void OnDeath()
    {
        Debug.Log("THE enemy DIED!");
        m_Animator.SetTrigger("DeathTrigger");
        m_AIBehaviour.Pause();

        EnableCollisions();
    }

    public void OnEndHitStun()
    {
        m_AIBehaviour.Resume();
    }

    private void EnableCollisions()
    {
        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].isKinematic = false;
        }
    }

    public void EnableRagdoll()
    {
        m_Animator.enabled = false;
    }

    
    //IMoveableObject
    public void AddVelocity(Vector3 velocity)
    {
        m_Animator.enabled = false;

        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].isKinematic = false;
        }

        m_MainRigidbody.AddForce(velocity);
        Debug.Log("GOT PUSHED BACK FOR " + velocity);
    }

    public Vector3 GetCenterOfMass()
    {
        return m_MainRigidbody.gameObject.transform.position;
    }
}
