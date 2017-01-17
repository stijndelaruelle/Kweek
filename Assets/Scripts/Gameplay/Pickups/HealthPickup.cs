using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasicPickup
{
    [Header("Health")]
    [SerializeField]
    private int m_Health;

    [SerializeField]
    private bool m_Overheal = true;

    public override void Pickup(Player player)
    {
        IDamageableObject damageableObject = player.DamageableObject;

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
