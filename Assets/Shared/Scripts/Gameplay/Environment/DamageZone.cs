using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField]
    private int m_DamagePerTick;

    [SerializeField]
    private float m_TickRate;

    //Keep lists in sync. Didn't feel like creating an extra struct to hold these together
    private List<float> m_Timers;
    private List<IDamageableObject> m_DamageableObjects;
    
    private void Awake()
    {
        m_Timers = new List<float>();
        m_DamageableObjects = new List<IDamageableObject>();
    }

    private void Update()
    {
        //Each object has their own timer
        for (int i = 0; i < m_Timers.Count; ++i)
        {
            m_Timers[i] += Time.deltaTime;

            if (m_Timers[i] > m_TickRate)
            {
                m_Timers[i] -= m_TickRate;

                IDamageableObject damageableObject = m_DamageableObjects[i];
                damageableObject.Damage(m_DamagePerTick);
               
                if (damageableObject.IsDead())
                {
                    RemoveDamageableObject(damageableObject);
                }
            }
        }
    }

    private void AddDamageableObject(IDamageableObject damageableObject)
    {
        if (damageableObject != null && damageableObject.IsDead() == false)
        {
            if (m_DamageableObjects.Contains(damageableObject) == false)
            {
                Debug.Log("Started damaging: " + damageableObject.gameObject.name);
                m_DamageableObjects.Add(damageableObject);
                m_Timers.Add(0.0f);
            }
        }
    }

    private void RemoveDamageableObject(IDamageableObject damageableObject)
    {
        if (damageableObject != null)
        {
            int index = m_DamageableObjects.IndexOf(damageableObject);
            if (index != -1)
            {
                Debug.Log("Stopped damaging: " + damageableObject.gameObject.name);

                m_DamageableObjects.RemoveAt(index);
                m_Timers.RemoveAt(index);
            }
        }
    }

    //Doesn't work when objects suddenly dissapear
    private void OnTriggerEnter(Collider collider)
    {
        IDamageableObject damageableObject = collider.gameObject.GetComponent<IDamageableObject>();
        AddDamageableObject(damageableObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        IDamageableObject damageableObject = collider.gameObject.GetComponent<IDamageableObject>();
        RemoveDamageableObject(damageableObject);
    }
}
