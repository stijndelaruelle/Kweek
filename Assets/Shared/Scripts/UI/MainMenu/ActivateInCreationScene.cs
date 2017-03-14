using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateInCreationScene : MonoBehaviour
{
    [SerializeField]
    private bool m_ActivateInCreationScene = true;

    [SerializeField]
    private bool m_ActivateInNonCreationScene = false;

    private string m_OriginalSceneName;

    private void Start()
    {
        m_OriginalSceneName = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        if (m_ActivateInCreationScene == false)
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene prevScene, Scene currentScene)
    {
        bool activate = false;

        if (currentScene.name == m_OriginalSceneName && m_ActivateInCreationScene == true)
        {
            activate = true;
        }

        if (currentScene.name != m_OriginalSceneName && m_ActivateInNonCreationScene == true)
        {
            activate = true;
        }

        gameObject.SetActive(activate);
    }
}
