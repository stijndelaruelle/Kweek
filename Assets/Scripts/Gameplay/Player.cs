using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private IDamageableObject m_DamageableObject;
    public IDamageableObject DamageableObject
    {
        get { return m_DamageableObject; }
    }

    [SerializeField]
    private WeaponArsenal m_WeaponArsenal;
    public WeaponArsenal WeaponArsenal
    {
        get { return m_WeaponArsenal; }
    }

    private void Start()
    {
        m_DamageableObject.DeathEvent += OnDeath;
    }

    private void OnDestroy()
    {
        if (m_DamageableObject != null)
            m_DamageableObject.DeathEvent -= OnDeath;
    }

    private void OnDeath()
    {
        Debug.Log("THE PLAYER DIED!");
    }
}
