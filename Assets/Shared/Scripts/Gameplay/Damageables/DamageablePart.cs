using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the part that has the collider
//It sends the damage to the main damageableobject
public class DamageablePart : IDamageableObject
{
    [SerializeField]
    private IDamageableObject m_MainObject;
    public IDamageableObject MainObject
    {
        get { return m_MainObject; }
    }

    public override int MaxHealth
    {
        get { return m_MainObject.MaxHealth; }
    }
    public override int Health
    {
        get { return m_MainObject.Health; }
    }


    [SerializeField]
    private float m_DamageMultiplier = 1.0f;

    public override int ChangeHealth(int health)
    {
        int reserveHealth = m_MainObject.ChangeHealth(health);
        CallChangeHealthEvent(health);

        return reserveHealth;
    }

    public override int Damage(int health)
    {
        int actualDamage = Mathf.CeilToInt(health * m_DamageMultiplier);

        int reserveDamage = m_MainObject.Damage(actualDamage);
        CallDamageEvent(actualDamage);

        return reserveDamage;
    }

    public override int Heal(int health)
    {
        int actualHealing = Mathf.CeilToInt(health * m_DamageMultiplier);

        int reserveHealth = m_MainObject.Heal(actualHealing);
        CallHealEvent(actualHealing);

        return reserveHealth;
    }

    public override bool IsDead()
    {
        return m_MainObject.IsDead();
    }

    public override Vector3 GetPosition()
    {
        return transform.position;
    }

    public override IDamageableObject GetMainDamageableObject()
    {
        return m_MainObject;
    }
}
