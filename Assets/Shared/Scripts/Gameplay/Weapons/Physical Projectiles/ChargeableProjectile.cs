using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Very related to PhysicalProjectile, see if we can merge this later
public delegate void FullyChargeDelegate();

public class ChargeableProjectile : MonoBehaviour
{
    [Header("Damage")]
    [Space(5)]
    [SerializeField]
    [MinMaxRange(0f, 100f)]
    [Tooltip("Min: Low charge / Max: High charge")]
    protected MinMaxRange m_DirectImpactDamage;

    [SerializeField]
    [MinMaxRange(0f, 100f)]
    [Tooltip("Min: Low charge / Max: High charge")]
    protected MinMaxRange m_ExplosionDamage;

    [SerializeField]
    [MinMaxRange(0f, 100f)]
    [Tooltip("Min: Low charge / Max: High charge")]
    protected MinMaxRange m_ExplosionRadius;

    [SerializeField]
    [MinMaxRange(0f, 10000f)]
    [Tooltip("Min: Low charge / Max: High charge")]
    protected MinMaxRange m_ExplosionForce;

    [Space(10)]
    [Header("Charging")]
    [Space(5)]
    [SerializeField]
    [MinMaxRange(0f, 100f)]
    [Tooltip("Uniform / Min: Low charge / Max: High charge")]
    private MinMaxRange m_Scale;

    [SerializeField]
    [MinMaxRange(0f, 100f)]
    [Tooltip("Min: Low charge / Max: High charge")]
    private MinMaxRange m_LifeTime;
    private float m_Counter;

    [SerializeField]
    [MinMaxRange(0f, 10000f)]
    [Tooltip("Min: High charge / Max: Low charge")]
    private MinMaxRange m_InitialSpeed;

    [SerializeField]
    [MinMaxRange(0f, 100f)]
    protected MinMaxRange m_ChargeLimit;

    [SerializeField]
    private Rigidbody m_Rigidbody;
    private bool m_IsCharging = false;
    private float m_ChargeTimer = 0.0f;
    public float ChargeTime
    {
        get { return m_ChargeTimer; }
    }

    public event FullyChargeDelegate FullChargedEvent;

    public void StartCharging()
    {
        //Make bigger
        float scale = m_Scale.Min;
        transform.localScale = new Vector3(scale, scale, scale);

        m_IsCharging = true;
        m_ChargeTimer = 0.0f;
    }

    public bool Fire(Vector3 direction, Vector3 baseVelocity)
    {
        if (CanFire() == false)
            return false;

        float speed = m_InitialSpeed.GetValue(1.0f - GetNormalizedChargeTime());
        Vector3 force = (direction * speed) + baseVelocity;
        m_Rigidbody.AddForce(force);

        m_IsCharging = false;
        return true;
    }


    private void Update()
    {
        HandleCharging();
        HandleLifeTime();
    }

    private void HandleCharging()
    {
        if (!m_IsCharging)
            return;

        //Make bigger
        float scale = m_Scale.GetValue(GetNormalizedChargeTime());
        transform.localScale = new Vector3(scale, scale, scale);
        //Use ammo callback?

        m_ChargeTimer += Time.deltaTime;
        if (m_ChargeTimer >= m_ChargeLimit.Max)
        {
            m_IsCharging = false;
            if (FullChargedEvent != null) //TODO use later.
                FullChargedEvent();
        }
    }

    private void HandleLifeTime()
    {
        m_Counter += Time.deltaTime;

        if (m_Counter >= m_LifeTime.GetValue(GetNormalizedChargeTime()))
        {
            Explode(null);
        }
    }

    protected void Explode(IDamageableObject directImpactTarget)
    {
        float directImpactDamage = m_DirectImpactDamage.GetValue(GetNormalizedChargeTime());
        float explosionDamage = m_ExplosionDamage.GetValue(GetNormalizedChargeTime());

        float explosionRadius = m_ExplosionRadius.GetValue(GetNormalizedChargeTime());
        float explosionForce = m_ExplosionForce.GetValue(GetNormalizedChargeTime());


        //Explosion damage & pushback
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        List<IDamageableObject> hitObjects = new List<IDamageableObject>();

        //Collect all the unique objects (objects can consist of multiple colliders)
        for (int i = 0; i < colliders.Length; ++i)
        {
            IDamageableObject root = null;

            IDamageableObject damageableObject = colliders[i].gameObject.GetComponent<IDamageableObject>();
            if (damageableObject != null) root = damageableObject.GetMainDamageableObject();

            if (root != null && !hitObjects.Contains(root))
            {
                hitObjects.Add(root);
            }

            //Did we hit a rigidbody?
            Rigidbody rigidBody = colliders[i].transform.gameObject.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                Vector3 direction = (colliders[i].bounds.center - transform.position);
                Vector3 normDirection = direction.normalized;

                float distance = direction.magnitude;
                float normDistance = distance / explosionRadius; //0 = super close, 1 = furthest away
                if (normDistance > 1.0f) normDistance = 1.0f;
                float inversedNormDistance = 1.0f - normDistance;  //1 = super close, 0 = furthest away (used as a multiplier)

                rigidBody.AddForceAtPosition(normDirection * (explosionForce * inversedNormDistance), colliders[i].bounds.center);
            }
        }

        //For each unique object do damage & push back calculations
        foreach (IDamageableObject damageableObject in hitObjects)
        {
            IMoveableObject moveableObject = damageableObject.GetComponent<IMoveableObject>();

            Vector3 centerOfMass = transform.position;
            if (moveableObject != null) centerOfMass = moveableObject.GetCenterOfMass();

            //Get the center position of 
            //Calculate distance for damage falloff & explosion force
            Vector3 direction = (centerOfMass - transform.position);
            Vector3 normDirection = direction.normalized;

            float distance = direction.magnitude;
            float normDistance = distance / explosionRadius; //0 = super close, 1 = furthest away
            if (normDistance > 1.0f) normDistance = 1.0f;
            float inversedNormDistance = 1.0f - normDistance;  //1 = super close, 0 = furthest away (used as a multiplier)

            //Did we hit a damageableObjet
            if (damageableObject != null) //directImpactTarget already had his fair share of damage
            {
                if (damageableObject == directImpactTarget)
                {
                    damageableObject.Damage(Mathf.CeilToInt(directImpactDamage));
                }
                else
                {
                    damageableObject.Damage(Mathf.CeilToInt(explosionDamage * inversedNormDistance));
                }
            }

            //Did we hit a moveableObject?
            if (moveableObject != null)
            {
                //Always fly up a bit (for fun)
                normDirection.y += 0.75f;
                normDirection.Normalize();

                moveableObject.AddVelocity(normDirection * (explosionForce * inversedNormDistance));
                Debug.Log(inversedNormDistance);
            }
        }

        Destroy(gameObject);
    }

    private float GetNormalizedChargeTime()
    {
        return (m_ChargeTimer / m_ChargeLimit.Max);
    }

    public bool CanFire()
    {
        return (m_ChargeTimer >= m_ChargeLimit.Min);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject root = collision.gameObject.transform.root.gameObject;

        //Direct hit
        IDamageableObject damageableObject = root.GetComponent<IDamageableObject>();
        Explode(damageableObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius.GetValue(GetNormalizedChargeTime()));
    }
}
