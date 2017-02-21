using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSwitchOnPlayerDeath : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private bool m_Enable;

    private void Start()
    {
        if (m_Player != null)
        {
            m_Player.DeathEvent += OnPlayerDeath;
            m_Player.RespawnEvent += OnPlayerRespawn;
        }

        gameObject.SetActive(!m_Enable);
    }

    private void OnDestroy()
    {
        if (m_Player != null)
        {
            m_Player.DeathEvent -= OnPlayerDeath;
            m_Player.RespawnEvent -= OnPlayerRespawn;
        }
    }

    private void OnPlayerDeath()
    {
        gameObject.SetActive(m_Enable);
    }

    private void OnPlayerRespawn()
    {
        gameObject.SetActive(!m_Enable);
    }
}
