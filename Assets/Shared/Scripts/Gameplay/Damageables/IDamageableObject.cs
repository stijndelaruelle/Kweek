using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DamageDelegate();
public delegate void HealDelegate();
public delegate void ChangeHealthDelegate(int health);
public delegate void DeathDelegate();

public abstract class IDamageableObject : MonoBehaviour
{
    //Events
    public event DamageDelegate DamageEvent;
    public event HealDelegate HealEvent;
    public event ChangeHealthDelegate ChangeHealthEvent;
    public event DeathDelegate DeathEvent;

    protected void CallDamageEvent()
    {
        if (DamageEvent != null)
            DamageEvent();
    }
    protected void CallHealEvent()
    {
        if (HealEvent != null)
            HealEvent();
    }
    protected void CallChangeHealthEvent(int health)
    {
        if (ChangeHealthEvent != null)
            ChangeHealthEvent(health);
    }
    protected void CallDeathEvent()
    {
        if (DeathEvent != null)
            DeathEvent();
    }


    public abstract int MaxHealth
    {
        get;
    }
    public abstract int Health
    {
        get;
    }

    public abstract int Damage(int health);
    public abstract void Heal(int health);

    public abstract bool IsDead();

    public abstract IDamageableObject GetMainDamageableObject();
}
