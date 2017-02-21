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

    public override int Damage(int health)
    {
        int reserveDamage = m_MainObject.Damage(Mathf.CeilToInt(health * m_DamageMultiplier));
        CallDamageEvent();

        return reserveDamage;
    }

    public override void Heal(int health)
    {
        m_MainObject.Heal(Mathf.CeilToInt(health * m_DamageMultiplier));
        CallHealEvent();
    }

    public override IDamageableObject GetMainDamageableObject()
    {
        return m_MainObject;
    }
}
