using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private IDamageableObject m_DamageableObject;

    private void Start()
    {
        m_DamageableObject.DeathEvent += OnDeath;
    }

    private void OnDeath()
    {
        Debug.Log("THE PLAYER DIED!");
    }
}
