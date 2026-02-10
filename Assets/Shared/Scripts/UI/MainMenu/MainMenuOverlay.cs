using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kweek
{
    public class MainMenuOverlay : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Visuals = null;
        private LevelManager m_LevelManager = null;

        private string m_OriginalSceneName = string.Empty;

        private void Start()
        {
            m_OriginalSceneName = SceneManager.GetActiveScene().name;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            m_LevelManager = LevelManager.Instance;
        }

        private void Update()
        {
            //Open and close the menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_LevelManager.IsCurrentLevelLoaded())
                {
                    SetVisible(!IsVisible());
                }
            }
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void SetVisible(bool state)
        {
            m_Visuals.SetActive(state);

            //Cursor & time state (TODO: Find another place to do this!)
            if (IsVisible())
            {
                Time.timeScale = 0.0f;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1.0f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private bool IsVisible()
        {
            return m_Visuals.activeSelf;
        }

        private void OnActiveSceneChanged(Scene prevScene, Scene currentScene)
        {
            bool activate = false;

            if (currentScene.name == m_OriginalSceneName)
            {
                activate = true;
            }

            SetVisible(activate);
        }
    }
}