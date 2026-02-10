using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kweek
{
    public class SceneLoader : MonoBehaviour
    {
        public delegate void SceneLoaderDelegate();

        private string m_SceneName = string.Empty;
        private LoadSceneMode m_Mode = LoadSceneMode.Single;
        private bool m_SetAsMainScene = false;

        private bool m_IsLoading = false;
        private bool m_IsActivated = false;
        private AsyncOperation m_AsyncProgress = null;

        public event SceneLoaderDelegate SceneLoadedEvent = null;
        public event SceneLoaderDelegate SceneActivatedEvent = null;

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


        public void LoadScene(string sceneName, LoadSceneMode mode, bool async, bool setAsMainScene)
        {
            if (m_IsLoading == true && m_IsActivated == false)
            {
                Debug.LogWarning("Trying to load 2 scenes simultainously!");
                return;
            }

            m_SceneName = sceneName;
            m_Mode = mode;
            m_SetAsMainScene = setAsMainScene;

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

                StartCoroutine(OnSceneActivatedFrameDelayRoutine(scene));
            }
        }

        private IEnumerator OnSceneActivatedFrameDelayRoutine(Scene scene)
        {
            //https://forum.unity3d.com/threads/scenemanager-setactivescene-does-not-work-solved-workarounds.381705/
            yield return new WaitForEndOfFrame();

            if (m_SetAsMainScene)
            {
                SceneManager.SetActiveScene(scene);
            }

            if (SceneActivatedEvent != null)
                SceneActivatedEvent();

            m_IsActivated = true;
            m_AsyncProgress = null;
        }
    }
}