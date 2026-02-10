using UnityEngine;

namespace Kweek
{
    public class QuitToMenuButton : MonoBehaviour
    {
        [SerializeField]
        private PopupPanel m_PopupWindow = null;

        [SerializeField]
        private ImageFader m_ImageFader = null;

        public void Quit()
        {
            m_PopupWindow.SetupYesNo("Quit", "Are you sure you want to quit?", OnYesClicked, null);
        }

        private void OnYesClicked()
        {
            SaveGameManager.Instance.DeactivateSaveGame();
            m_ImageFader.FadeIn(OnFadeInComplete);
        }

        private void OnFadeInComplete()
        {
            LevelManager.Instance.LoadMainMenu();
            m_ImageFader.SetAlphaMin();
        }
    }
}