using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverhealDamageBehaviour : RegularDamageBehaviour
{
    [Header("Overheal Settings")]
    [Space(5)]
    [SerializeField]
    private int m_MaxOverhealHealth;

    [SerializeField]
    [Tooltip("Time it takes for 1 hitpoint to dissapear.")]
    private float m_OverhealDecayRate;
    private float m_OverhealDecayTimer;

    private void Update()
    {
        //Overheal decay
        if (m_Health > m_MaxHealth)
        {
            m_OverhealDecayTimer += Time.deltaTime;

            if (m_OverhealDecayTimer > m_OverhealDecayRate)
            {
                m_OverhealDecayTimer -= m_OverhealDecayRate;
                ChangeHealth(-1);
            }

            if (m_Health <= m_MaxHealth)
            {
                m_OverhealDecayTimer = 0.0f;
            }
        }
    }

    public override int ChangeHealth(int health)
    {
        int prevHealth = m_Health;
        m_Health += health;

        int reserveHealth = 0;

        if (m_Health > m_MaxOverhealHealth)
        {
            reserveHealth = m_Health - m_MaxOverhealHealth;
            m_Health = m_MaxOverhealHealth;
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
        if (m_Health != prevHealth)
            CallChangeHealthEvent(m_Health);

        return reserveHealth;
    }
}
