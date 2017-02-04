using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IMoveableObject
{
    [SerializeField]
    private IDamageableObject m_DamageableObject;

    [SerializeField]
    private WeaponPickup m_WeaponPickup;

    //[SerializeField]
    //private IAIBehaviour m_AIBehaviour; //The time will come...
    [SerializeField]
    private SoldierBehaviour m_AIBehaviour;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Rigidbody m_MainRigidbody;
    public Rigidbody MainRigidbody
    {
        get { return m_MainRigidbody; }
    }

    //Get component instead of assigning, because assigning manually is very error prone
    private Rigidbody[] m_Rigidbodies;

    private RagdollPart[] m_RagdollParts;
    public RagdollPart[] RagdollParts
    {
        get { return m_RagdollParts; }
    }

    private List<Collider> m_Colliders;
    public List<Collider> Colliders
    {
        get { return m_Colliders; }
    }

    private void Awake()
    {
        m_Colliders = new List<Collider>();
        m_Colliders.AddRange(GetComponents<Collider>());
        m_Colliders.AddRange(GetComponentsInChildren<Collider>());

        //Enable kinematic (otherwise raycasts will occasionally miss!)
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
        m_RagdollParts = GetComponentsInChildren<RagdollPart>();

        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].isKinematic = true;
        }

        //Enable our animator
        m_Animator.enabled = true;
    }

    private void Start()
    {
        m_DamageableObject.DamageEvent += OnDamage;
        m_DamageableObject.DeathEvent += OnDeath;

        m_WeaponPickup.enabled = false;
        m_AIBehaviour.Setup(m_Colliders);
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
            //m_Animator.SetTrigger("WoundTrigger");
            //m_AIBehaviour.Pause();
        }
    }

    private void OnDeath()
    {
        Debug.Log("THE enemy DIED!");
        //m_Animator.SetTrigger("DeathTrigger");
        m_WeaponPickup.gameObject.transform.parent = null;
        m_WeaponPickup.enabled = true;

        m_AIBehaviour.Pause();
        m_AIBehaviour.enabled = false;

        EnableCollisions();
        EnableRagdoll();
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

    public bool IsRagdollEnabled()
    {
        return (!m_Animator.enabled);
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
