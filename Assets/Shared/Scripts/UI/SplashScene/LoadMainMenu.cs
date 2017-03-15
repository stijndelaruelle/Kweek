using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    [SerializeField]
    private SceneLoader m_SceneLoader;

    [SerializeField]
    private string m_MainMenuName;

    [SerializeField]
    private string m_MainMenuBackgroundName;
    private bool m_LoadingBackground = false;

    private void Start()
    {
        //Cool splash fades etc...

        m_SceneLoader.SceneActivatedEvent += OnSceneActivated;

        //Load the main menu first
        m_LoadingBackground = true;
        m_SceneLoader.LoadScene(m_MainMenuBackgroundName, UnityEngine.SceneManagement.LoadSceneMode.Additive, true, true);
        m_SceneLoader.ActivateScene();
    }

    private void OnSceneActivated()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (m_LoadingBackground == true)
        {
            m_SceneLoader.LoadScene(m_MainMenuName, UnityEngine.SceneManagement.LoadSceneMode.Additive, true, false);
            m_SceneLoader.ActivateScene();
            m_LoadingBackground = false;
            return;
        }

        if (m_LoadingBackground == false)
        {
            //Everything is loaded, remove blackness
        }
    }
}
