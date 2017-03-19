using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : MonoBehaviour
{
    [SerializeField]
    private LevelSelectPanel m_LevelTSelectPanel;

    [SerializeField]
    private DifficultySelectPanel m_DifficultySelectPanel;

    [SerializeField]
    private ImageFader m_ImageFader;

    public void StartNewGame()
    {
        //Difficulty mode
        int levelID = LevelManager.Instance.GetLevelID(m_LevelTSelectPanel.SelectedLevelData);
        int difficulty = m_DifficultySelectPanel.SelectedDifficulty;

        SaveGame saveGame = SaveGameManager.Instance.CreateSaveGame("My Save Game", difficulty, levelID, 0);

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
