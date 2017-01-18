using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Visuals;

    [SerializeField]
    private Text m_PickupText;

    [SerializeField]
    private PickupHandler m_PickupHandler;

    private void Awake()
    {
        m_PickupHandler.ChangePickupEvent += OnChangePickup;
        m_Visuals.SetActive(false);
    }

    private void OnDestroy()
    {
        if (m_PickupHandler != null)
            m_PickupHandler.ChangePickupEvent -= OnChangePickup;
    }

    public void OnChangePickup(IPickup pickup)
    {
        if (pickup == null)
        {
            m_Visuals.SetActive(false);
            return;
        }

        m_Visuals.SetActive(true);
        m_PickupText.text = "Press [E] to pickup " + pickup.PickupName + ".";
    }
}