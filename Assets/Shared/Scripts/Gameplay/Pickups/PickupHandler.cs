using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private float m_Range;
    private bool m_IsEnabled = true;

    //Event
    public event ChangePickupDelegate ChangePickupEvent;

    private void Start()
    {
        m_Player.DeathEvent += OnPlayerDeath;
        m_Player.RespawnEvent += OnPlayerRespawn;
    }

    private void OnDestroy()
    {
        if (m_Player == null)
            return;

        m_Player.DeathEvent -= OnPlayerDeath;
        m_Player.RespawnEvent -= OnPlayerRespawn;
    }

    private void Update()
    {
        if (!m_IsEnabled)
            return;

        //Fire a single ray (get only the first target)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        RaycastHit hitInfo;
        bool success = Physics.Raycast(ray, out hitInfo, m_Range);

        Debug.DrawRay(ray.origin, ray.direction * m_Range, Color.red);

        if (!success)
        {
            FireChangePickupEvent(null);
            return;
        }

        GameObject go = hitInfo.collider.gameObject;

        //Did we hit a pickup?
        IPickup pickup = go.GetComponent<IPickup>();

        if (pickup != null)
        {
            if (Input.GetButtonDown("Use")) //Input.GetKeyDown(KeyCode.E)
            {
                pickup.Pickup(m_Player);
            }
        }

        FireChangePickupEvent(pickup);
    }

    private void FireChangePickupEvent(IPickup pickup)
    {
        if (ChangePickupEvent != null)
            ChangePickupEvent(pickup);
    }

    private void OnPlayerDeath()
    {
        m_IsEnabled = false;
        FireChangePickupEvent(null);
    }

    private void OnPlayerRespawn()
    {
        m_IsEnabled = true;
    }
}
