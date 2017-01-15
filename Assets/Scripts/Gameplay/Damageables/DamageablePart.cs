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

    //Events
    protected DamageDelegate m_DamageEvent;
    public override DamageDelegate DamageEvent
    {
        get { return m_DamageEvent; }
        set { m_DamageEvent = value; }
    }

    protected HealDelegate m_HealEvent;
    public override HealDelegate HealEvent
    {
        get { return m_HealEvent; }
        set { m_HealEvent = value; }
    }

    protected ChangeHealthDelegate m_ChangeHealthEvent;
    public override ChangeHealthDelegate ChangeHealthEvent
    {
        get { return m_ChangeHealthEvent; }
        set { m_ChangeHealthEvent = value; }
    }

    protected DeathDelegate m_DeathEvent;
    public override DeathDelegate DeathEvent
    {
        get { return m_DeathEvent; }
        set { m_DeathEvent = value; }
    }


    [SerializeField]
    private float m_DamageMultiplier = 1.0f;

    public override void Damage(int health)
    {
        m_MainObject.Damage(Mathf.CeilToInt(health * m_DamageMultiplier));

        if (m_DamageEvent != null)
            m_DamageEvent();
    }

    public override void Heal(int health)
    {
        m_MainObject.Heal(Mathf.CeilToInt(health * m_DamageMultiplier));

        if (m_HealEvent != null)
            m_HealEvent();
    }

    public override IDamageableObject GetMainDamageableObject()
    {
        return m_MainObject;
    }
}
