using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Refactor this class (split it up)
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private int m_NumberOfProjectiles;

    [Tooltip("Max spread at max range.")]
    [SerializeField]
    private float m_Spread;

    [SerializeField]
    private float m_Range;

    [SerializeField]
    private Transform m_ProjectileSpawn;

    [SerializeField]
    private GameObject m_Projectile;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private PlayerController m_PlayerController;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    public void Fire()
    {
        m_Animator.SetTrigger("FireTrigger");

        Ray centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        Vector3 forward = centerRay.direction;
        Vector3 right = new Vector3(-forward.z, 0.0f, forward.x);
        Vector3 up = Vector3.Cross(right, forward);

        for (int i = 0; i < m_NumberOfProjectiles; ++i)
        {
            Ray ray = centerRay;
            float range = m_Range;

            if (m_Spread > 0.0f)
            {
                Vector3 maxPosition = centerRay.direction * m_Range;

                maxPosition += right * Random.Range(-m_Spread, m_Spread);
                maxPosition += up * Random.Range(-m_Spread, m_Spread);

                ray.direction = maxPosition.normalized;
                range = maxPosition.magnitude;
            }

            //Can't instantiate interfaces. Preferred this setup above an abstract class. (it's overall more consistent, but a little bit messier here)
            GameObject go = GameObject.Instantiate<GameObject>(m_Projectile, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation);
            IProjectile projectile = go.GetComponent<IProjectile>();

            if (projectile != null)
            {
                projectile.Fire(m_PlayerController.CurrentVelocity, ray, range);
            }
        }
    }
}
