using UnityEngine;

namespace Kweek
{
    public class QuitToDesktopButton : MonoBehaviour
    {
        [SerializeField]
        private PopupPanel m_PopupWindow = null;

        public void Quit()
        {
            m_PopupWindow.SetupYesNo("Quit", "Are you sure you want to quit?", OnYesClicked, null);
        }

        private void OnYesClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}