using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : MonoBehaviour
{
    [SerializeField]
    private LevelSelectPanel m_LevelToggleGroup;

    [SerializeField]
    private ImageFader m_ImageFader;

    public void StartNewGame()
    {
        m_ImageFader.FadeIn(OnFadeInComplete);
    }

    private void OnFadeInComplete()
    {
        LevelManager.Instance.LoadLevel(m_LevelToggleGroup.SelectedLevelData);
        m_ImageFader.SetAlphaMin();
    }
}
