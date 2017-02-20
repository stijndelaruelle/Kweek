using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    [SerializeField]
    private PopupPanel m_PopupWindow;

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
