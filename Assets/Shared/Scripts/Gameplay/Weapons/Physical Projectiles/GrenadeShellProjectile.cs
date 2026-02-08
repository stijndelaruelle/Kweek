using UnityEngine;

namespace Kweek
{
    public class GrenadeShellProjectile : PhysicalProjectile
    {
        private bool m_HasBounced = false;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject root = collision.gameObject.transform.root.gameObject;

            //Direct hit
            IDamageableObject damageableObject = root.GetComponent<IDamageableObject>();

            if (damageableObject != null && m_HasBounced == false)
                Explode(damageableObject);

            m_HasBounced = true;
        }
    }
}