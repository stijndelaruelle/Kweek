using UnityEngine;

namespace Kweek
{
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
            if (SaveGameManager.Instance.ActiveSaveGame != null)
            {
                LevelManager.Instance.LoadLevel(SaveGameManager.Instance.ActiveSaveGame.LevelID);
                m_ImageFader.SetAlphaMin();
            }
        }
    }
}