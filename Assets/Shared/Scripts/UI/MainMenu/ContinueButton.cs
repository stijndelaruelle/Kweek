using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    [SerializeField]
    private ImageFader m_ImageFader;

    public void Continue()
    {
        SaveGameManager.Instance.ActivateMostRecent();
        m_ImageFader.FadeIn(OnFadeInComplete);
    }

    private void OnFadeInComplete()
    {
        LevelManager.Instance.LoadLevel(SaveGameManager.Instance.ActiveSaveGame.LevelID);
        m_ImageFader.SetAlphaMin();
    }
}
