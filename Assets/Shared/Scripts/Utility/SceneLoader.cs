using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public delegate void SceneLoaderDelegate();

    private string m_SceneName;
    private LoadSceneMode m_Mode;

    private bool m_IsLoading = false;
    private bool m_IsActivated = false;
    private AsyncOperation m_AsyncProgress = null;

    public event SceneLoaderDelegate SceneLoadedEvent;
    public event SceneLoaderDelegate SceneActivatedEvent;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneActivated;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneActivated;
    }

    private void Update()
    {
        //Async loading can only be done up until 90%.
        if (m_AsyncProgress != null && m_IsLoading)
        {
            float progress = GetProgress();

            if (progress >= 1.0f)
            {
                OnSceneLoaded();
            }
        }
    }


    public void LoadScene(string sceneName, LoadSceneMode mode, bool async)
    {
        if (m_IsLoading == true && m_IsActivated == false)
        {
            Debug.LogWarning("Trying to load 2 scenes simultainously!");
            return;
        }

        m_SceneName = sceneName;
        m_Mode = mode;

        if (async)
        {
            m_AsyncProgress = SceneManager.LoadSceneAsync(m_SceneName, m_Mode);
            m_AsyncProgress.allowSceneActivation = false;
        }
        else
        {
            SceneManager.LoadScene(m_SceneName, m_Mode);
        }

        m_IsLoading = true;
        m_IsActivated = false;
    }

    public float GetProgress()
    {
        if (m_AsyncProgress == null)
        {
            if (m_IsLoading)
                return 0.0f;
            else
                return 1.0f;
        }

        return m_AsyncProgress.progress / 0.9f; //Async loading only get's up to 0.9
    }

    public void ActivateScene()
    {
        if (m_AsyncProgress != null)
            m_AsyncProgress.allowSceneActivation = true;
    }


    private void OnSceneLoaded()
    {
        if (SceneLoadedEvent != null)
            SceneLoadedEvent();

        m_IsLoading = false;
    }

    private void OnSceneActivated(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == m_SceneName && loadMode == m_Mode)
        {
            if (m_AsyncProgress == null)
                OnSceneLoaded();

            if (SceneActivatedEvent != null)
                SceneActivatedEvent();

            m_IsActivated = true;
            m_AsyncProgress = null;
        }
    }
}
