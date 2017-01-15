using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the part that has the collider
//It sends the damage to the main damageableobject
public class DamageablePart : MonoBehaviour, IDamageableObject
{
    [SerializeField]
    private PatrollingEnemy m_MainObject; //Make this more generic later, Unity doesn't know what to do with interfaces.
    public GameObject MainObject
    {
        get { return m_MainObject.gameObject; }
    }

    [SerializeField]
    private float m_DamageMultiplier;

    public void Damage(int health)
    {
        m_MainObject.Damage(Mathf.CeilToInt(health * m_DamageMultiplier));
    }

    public IDamageableObject GetMainDamageableObject()
    {
        return m_MainObject;
    }
}
