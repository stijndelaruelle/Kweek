using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutOnLevelLoad : MonoBehaviour
{
    [SerializeField]
    private ImageFader m_ImageFader;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FadeOut();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeOut();
    }

    private void FadeOut()
    {
        m_ImageFader.SetAlphaMax();
        m_ImageFader.FadeOut();
    }
}
