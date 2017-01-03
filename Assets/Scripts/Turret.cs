using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    [SerializeField]
    private Gun m_Gun;

    [SerializeField]
    private float m_FireRate = 1.0f;
    private float m_Timer = 0.0f;

    private void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer > m_FireRate)
        {
            Ray ray = new Ray(m_Gun.gameObject.transform.position, m_Gun.gameObject.transform.forward);
            m_Gun.Fire(ray);

            m_Timer = 0.0f;
        }
    }
}
