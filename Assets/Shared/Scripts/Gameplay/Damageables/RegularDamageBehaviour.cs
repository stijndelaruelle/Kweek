using System;
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

    private void Start()
    {
        ChangeHealth(m_MaxHealth);
    }

    public override int ChangeHealth(int health)
    {
        int prevHealth = m_Health;
        m_Health += health;

        int reserveHealth = 0;
        if (m_Health > m_MaxHealth)
        {
            reserveHealth = m_Health - m_MaxHealth;
            m_Health = m_MaxHealth;
        }

        if (m_Health <= 0)
        {
            reserveHealth = Mathf.Abs(m_Health);
            m_Health = 0;

            //Fire death event
            if (prevHealth != 0)
                CallDeathEvent();
        }

        //Fire healthchange event
        CallChangeHealthEvent(m_Health);

        return reserveHealth;
    }

    public override int Damage(int health)
    {
        if (IsDead())
            return health;

        int reserveHealth = ChangeHealth(-health);

        //Fire damage event
        CallDamageEvent(health);

        return reserveHealth;
    }

    public override int Heal(int health)
    {
        if (IsDead())
            return health;

        int reserveHealth = ChangeHealth(health);

        //Fire heal event
        CallHealEvent(health);

        return reserveHealth;
    }

    public override bool IsDead()
    {
        return (m_Health <= 0);
    }

    public override IDamageableObject GetMainDamageableObject()
    {
        return this;
    }
}
