using UnityEngine;
using System.Collections;

//Old hitscan weapon class
public class Gun : MonoBehaviour
{
    //Simple hitscan weapon
    [SerializeField]
    private float m_ReloadTime = 1.0f;
    private bool m_IsReloading = false;
    private Coroutine m_ReloadRoutine = null;

    [SerializeField]
    private float m_Range = 1000.0f;

    [SerializeField]
    private float m_Force = 5000.0f;

    [SerializeField]
    private GameObject m_Effect = null;

    public void Fire(Ray ray)
    {
        if (!m_IsReloading)
        {
            //Fire it
            RaycastHit raycastHit;
            bool success = Physics.Raycast(ray, out raycastHit, m_Range);

            //the first object hit should fly away
            if (success)
            {
                Rigidbody otherRigidbody = raycastHit.rigidbody;
                
                if (otherRigidbody != null)
                {
                    Debug.Log("HIT: " + otherRigidbody.name, otherRigidbody);

                    Rigidbody centerRigidBody = otherRigidbody;

                    //Check if we are a ragdollEnemy, if so: new center of mass & disable animations
                    Transform parent = otherRigidbody.transform;
                    PatrollingEnemy ragdollEnemy = parent.gameObject.GetComponentInParent<PatrollingEnemy>();

                    while (ragdollEnemy == null && parent != null)
                    {
                        parent = parent.transform.parent;
                        if (parent != null) ragdollEnemy = parent.gameObject.GetComponentInParent<PatrollingEnemy>();
                    }

                    if (ragdollEnemy != null)
                    {
                        //ragdollEnemy.OnHit();
                        centerRigidBody = ragdollEnemy.MainRigidbody;
                    }

                    Vector3 reflectDirection = centerRigidBody.position - raycastHit.point;
                    reflectDirection.Normalize();

                    otherRigidbody.AddForceAtPosition(reflectDirection * m_Force, raycastHit.point);
                }
            }

            //Show the effect
            m_Effect.SetActive(true);

            //Start reloading
            if (m_ReloadRoutine != null)
                StopCoroutine(m_ReloadRoutine);

            m_ReloadRoutine = StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        m_IsReloading = true;

        yield return new WaitForSeconds(m_ReloadTime);

        m_IsReloading = false;
    }
}
