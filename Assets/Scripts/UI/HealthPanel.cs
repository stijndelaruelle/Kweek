using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanel : MonoBehaviour
{
    [SerializeField]
    private Text m_HealthText;

    [SerializeField]
    private IDamageableObject m_DamageableObject;

    private void Awake()
    {
        m_DamageableObject.ChangeHealthEvent += OnUpdateHealth;
    }

    public void OnUpdateHealth(int health)
    {
        m_HealthText.text = health.ToString();
    }
}