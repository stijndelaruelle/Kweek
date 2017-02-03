using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DamageDelegate();
public delegate void HealDelegate();
public delegate void ChangeHealthDelegate(int health);
public delegate void DeathDelegate();

public abstract class IDamageableObject : MonoBehaviour
{
    public abstract int MaxHealth
    {
        get;
    }
    public abstract int Health
    {
        get;
    }

    public abstract DamageDelegate DamageEvent
    {
        get;
        set;
    }
    public abstract HealDelegate HealEvent
    {
        get;
        set;
    }
    public abstract ChangeHealthDelegate ChangeHealthEvent
    {
        get;
        set;
    }
    public abstract DeathDelegate DeathEvent
    {
        get;
        set;
    }

    public abstract int Damage(int health);
    public abstract void Heal(int health);
    public abstract IDamageableObject GetMainDamageableObject();
}
