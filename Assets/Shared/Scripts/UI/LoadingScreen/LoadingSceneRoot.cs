using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneRoot : MonoBehaviour
{
    [Header("Scene references")]
    [Space(10)]
    private string m_SceneName;

    [SerializeField]
    private SceneLoader m_SceneLoader;

    [Space(5)]
    [Header("Fader")]
    [Space(10)]
    [SerializeField]
    private ImageFader m_ImageFader;

    [Space(5)]
    [Header("Panels")]
    [Space(10)]
    [SerializeField]
    private GameObject m_LoadingPanel;

    [SerializeField]
    private GameObject m_LoadedPanel;
    private bool m_HasSceneLoaded = false;

    private void Start()
    {
        m_SceneName = LevelManager.Instance.GetCurrentLevelData().SceneName;

        m_SceneLoader.SceneLoadedEvent    += OnSceneLoaded;
        m_SceneLoader.SceneActivatedEvent += OnSceneActivated;

        ShowLoadingPanel();
        m_SceneLoader.LoadScene(m_SceneName, LoadSceneMode.Single, true);

        m_ImageFader.SetAlphaMax();
        m_ImageFader.FadeOut();
    }

    private void OnDestroy()
    {
        if (m_SceneLoader != null)
        {
            m_SceneLoader.SceneLoadedEvent    -= OnSceneLoaded;
            m_SceneLoader.SceneActivatedEvent -= OnSceneActivated;
        }
    }

    private void Update()
    {
        //Button bypass
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ActivateScene();
        }
    }

    //Button callback
    public void ActivateScene()
    {
        if (m_HasSceneLoaded == true)
            m_ImageFader.FadeIn(OnFadeInComplete);
    }

    //SceneLoader callbacks
    private void OnSceneLoaded()
    {
        ShowLoadedPanel();
        m_HasSceneLoaded = true;
    }

    private void OnSceneActivated()
    {
        Debug.Log(m_SceneName + " has activated!");
    }

    //Fader callback
    private void OnFadeInComplete()
    {
        //Unload the loading scene
        m_SceneLoader.ActivateScene();
    }

    //Utility
    private void ShowLoadingPanel()
    {
        m_LoadingPanel.SetActive(true);
        m_LoadedPanel.SetActive(false);
    }

    private void ShowLoadedPanel()
    {
        m_LoadingPanel.SetActive(false);
        m_LoadedPanel.SetActive(true);
    }
}
