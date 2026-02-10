using UnityEngine;

namespace Kweek
{
    public class NewGamePanel : MonoBehaviour
    {
        [SerializeField]
        private LevelSelectPanel m_LevelTSelectPanel = null;

        [SerializeField]
        private DifficultySelectPanel m_DifficultySelectPanel = null;

        [SerializeField]
        private ImageFader m_ImageFader = null;

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
}