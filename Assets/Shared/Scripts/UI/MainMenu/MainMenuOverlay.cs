﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuOverlay : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Visuals;
    private LevelManager m_LevelManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        m_LevelManager = LevelManager.Instance;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        //Open and close the menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_LevelManager.IsCurrentLevelLoaded())
            {
                SetVisible(!IsVisible());
            }
        }
    }

    public void Show()
    {
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool state)
    {
        m_Visuals.SetActive(state);

        //Cursor & time state (TODO: Find another place to do this!)
        if (IsVisible())
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private bool IsVisible()
    {
        return m_Visuals.activeSelf;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Hide();
    }

}
