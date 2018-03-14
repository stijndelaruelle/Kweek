using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DamageDelegate(int removedHealth);
public delegate void HealDelegate(int addedHealth);
public delegate void ChangeHealthDelegate(int currentHeath);
public delegate void DeathDelegate();

public abstract class IDamageableObject : MonoBehaviour
{
    //Events
    public event DamageDelegate DamageEvent;
    public event HealDelegate HealEvent;
    public event ChangeHealthDelegate ChangeHealthEvent;
    public event DeathDelegate DeathEvent;

    protected void CallDamageEvent(int removedHealth)
    {
        if (DamageEvent != null)
            DamageEvent(removedHealth);
    }
    protected void CallHealEvent(int addedHealth)
    {
        if (HealEvent != null)
            HealEvent(addedHealth);
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

    public abstract int ChangeHealth(int health);
    public abstract int Damage(int health);
    public abstract int Heal(int health);

    public abstract bool IsDead();
    public abstract Vector3 GetPosition(); //The point where you can aim at as using .trasform.position may be in the wrong place (for good reason).

    public abstract IDamageableObject GetMainDamageableObject();
}
