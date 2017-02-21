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
    private bool m_HasDied;

    private void Start()
    {
        ChangeHealth(m_MaxHealth);
    }

    protected virtual int ChangeHealth(int health)
    {
        if (m_HasDied)
            return health;

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
            CallDeathEvent();
            m_HasDied = true;
        }

        //Fire healthchange event
        CallChangeHealthEvent(m_Health);

        return reserveHealth;
    }

    public override int Damage(int health)
    {
        if (m_HasDied)
            return health;

        int reserveHealth =  ChangeHealth(-health);

        //Fire damage event
        CallDamageEvent();

        return reserveHealth;
    }

    public override void Heal(int health)
    {
        if (m_HasDied)
            return;

        ChangeHealth(health);

        //Fire heal event
        CallHealEvent();
    }
 
    public override IDamageableObject GetMainDamageableObject()
    {
        return this;
    }
}
