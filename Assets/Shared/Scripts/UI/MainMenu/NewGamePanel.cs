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
        //Difficulty mode
        int levelID = LevelManager.Instance.GetLevelID(m_LevelToggleGroup.SelectedLevelData);

        SaveGame saveGame = SaveGameManager.Instance.CreateSaveGame("New game - Difficulty mode", levelID);

        if (saveGame != null)
        {
            SaveGameManager.Instance.ActivateSaveGame(saveGame);
            m_ImageFader.FadeIn(OnFadeInComplete);
        }
    }

    private void OnFadeInComplete()
    {
        LevelManager.Instance.LoadLevel(SaveGameManager.Instance.ActiveSaveGame.LevelID);
        m_ImageFader.SetAlphaMin();
    }
}
