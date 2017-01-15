using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]
    private int m_Health;

    [SerializeField]
    private bool m_Overheal = true;


    private void OnCollisionEnter(Collision collision)
    {
        IDamageableObject damageableObject = collision.gameObject.GetComponent<IDamageableObject>();

        if (damageableObject != null)
        {
            int addedHealth = m_Health;

            if (m_Overheal == false)
            {
                int diff = damageableObject.MaxHealth - damageableObject.Health;
                if (diff < addedHealth)
                    addedHealth = diff;
            }

            if (addedHealth > 0)
            {
                damageableObject.Heal(addedHealth);
                Destroy(gameObject);
            }
        }
    }
}
