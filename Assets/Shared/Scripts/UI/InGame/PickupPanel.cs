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

    private void Start()
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

        //TODO: Not actually bound to a key yet.
        m_PickupText.text = "Press [F] to pickup " + pickup.PickupName + ".";
    }
}