using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kweek
{
    public class FadeOutOnLevelLoad : MonoBehaviour
    {
        [SerializeField]
        private ImageFader m_ImageFader = null;

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
}