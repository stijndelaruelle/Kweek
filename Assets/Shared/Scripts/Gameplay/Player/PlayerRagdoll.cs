using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : Ragdoll
{
    [SerializeField]
    private Player m_Player;

    private void Start()
    {
        if (m_Player != null)
        {
            m_Player.DeathEvent += OnPlayerDeath;
            m_Player.RespawnEvent += OnPlayerRespawn;
        }

        gameObject.SetActive(false);
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
        SetParent(null);
        gameObject.SetActive(true);
    }

    private void OnPlayerRespawn()
    {
        SetParent(m_Player.transform);
        SetTransform(m_Player.transform);
        Reset();
        gameObject.SetActive(false);
    }
}
