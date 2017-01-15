using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularDamageBehaviour : IDamageableObject
{
    [SerializeField]
    protected int m_MaxHealth;
    public override int MaxHealth
    {
        get { return m_MaxHealth; }
    }

    protected int m_Health;
    public override int Health
    {
        get { return m_Health; }
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

    private void Start()
    {
        ChangeHealth(m_MaxHealth);
    }

    protected virtual void ChangeHealth(int health)
    {
        m_Health += health;

        if (m_Health > m_MaxHealth)
            m_Health = m_MaxHealth;

        if (m_Health <= 0)
        {
            m_Health = 0;

            //Fire death event
            if (m_DeathEvent != null)
                m_DeathEvent();
        }

        //Fire healthchange event
        if (m_ChangeHealthEvent != null)
            m_ChangeHealthEvent(m_Health);
    }

    public override void Damage(int health)
    {
        ChangeHealth(-health);

        //Fire damage event
        if (m_DamageEvent != null)
            m_DamageEvent();
    }

    public override void Heal(int health)
    {
        ChangeHealth(health);

        //Fire heal event
        if (m_HealEvent != null)
            m_HealEvent();
    }

    public override IDamageableObject GetMainDamageableObject()
    {
        return this;
    }
}
