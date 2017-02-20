﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour
{
    public delegate void PopupCallback();

    [Flags]
    public enum PopupButton
    {
        None = 0,
        Yes = 1,
        No = 2,
        Cancel = 4
    }

    [SerializeField]
    private Text m_Title;

    [SerializeField]
    private Text m_Description;

    //NOTE: Not extensible for now as I don't expect any new buttons to appear very soon.
    //If they do, make this an array. (Current method favours readablility)
    [SerializeField]
    private Button m_YesButton;
    private GameObject m_YesButtonGO;

    [SerializeField]
    private Button m_NoButton;
    private GameObject m_NoButtonGO;

    [SerializeField]
    private Button m_CancelButton;
    private GameObject m_CancelButtonGO;

    //Callbakcs
    private PopupCallback m_YesCallback;
    private PopupCallback m_NoCallback;
    private PopupCallback m_CancelCallback;

    private void Awake()
    {
        m_YesButtonGO = m_YesButton.gameObject;
        m_NoButtonGO = m_NoButton.gameObject;
        m_CancelButtonGO = m_CancelButton.gameObject;
    }

    //Setup
    public void SetupYesNo(string title, string description, PopupCallback yesCallback, PopupCallback noCallback)
    {
        Setup(title, description, PopupButton.Yes | PopupButton.No, yesCallback, noCallback, null);
    }

    public void Setup(string title, string description, PopupButton buttons, PopupCallback yesCallback, PopupCallback noCallback, PopupCallback cancelCallback)
    {
        Show();

        m_Title.text = title;
        m_Description.text = description;

        //Disable all the buttons
        bool yesButtonState = false;
        bool noButtonState = false;
        bool cancelButtonState = false;

        //Enable required buttons
        if ((buttons & PopupButton.Yes) == PopupButton.Yes)       { yesButtonState = true; }
        if ((buttons & PopupButton.No) == PopupButton.No)         { noButtonState = true; }
        if ((buttons & PopupButton.Cancel) == PopupButton.Cancel) { cancelButtonState = true; }

        m_YesButtonGO.SetActive(yesButtonState);
        m_NoButtonGO.SetActive(noButtonState);
        m_CancelButtonGO.SetActive(cancelButtonState);

        //Setup callbakcs
        m_YesCallback = yesCallback;
        m_NoCallback = noCallback;
        m_CancelCallback = cancelCallback;
    }

    //Utility
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    //UI button callbacks
    public void YesClicked()
    {
        if (m_YesCallback != null)
            m_YesCallback();

        Hide();
    }

    public void NoClicked()
    {
        if (m_NoCallback != null)
            m_NoCallback();

        Hide();
    }

    public void CancelClicked()
    {
        if (m_CancelCallback != null)
            m_CancelCallback();

        Hide();
    }
}
