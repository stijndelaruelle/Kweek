using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private float m_Range;

    //Event
    private ChangePickupDelegate m_ChangePickupEvent;
    public ChangePickupDelegate ChangePickupEvent
    {
        get { return m_ChangePickupEvent; }
        set { m_ChangePickupEvent = value; }
    }

    private void Update()
    {
        //Fire a single ray (get only the first target)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        RaycastHit hitInfo;
        bool succes = Physics.Raycast(ray, out hitInfo, m_Range);

        Debug.DrawRay(ray.origin, ray.direction * m_Range, Color.red);

        if (!succes)
        {
            FireChangePickupEvent(null);
            return;
        }

        GameObject go = hitInfo.collider.gameObject;

        //Did we hit a pickup?
        IPickup pickup = go.GetComponent<IPickup>();

        if (pickup != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pickup.Pickup(m_Player);
            }
        }

        FireChangePickupEvent(pickup);
    }

    private void FireChangePickupEvent(IPickup pickup)
    {
        if (m_ChangePickupEvent != null)
            m_ChangePickupEvent(pickup);
    }
}
