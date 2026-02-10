using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kweek
{
    public class TabSwitchPanel : MonoBehaviour
    {
        [SerializeField]
        private List<Button> m_Buttons = null;

        [SerializeField]
        private List<GameObject> m_Panels = null;

        [SerializeField]
        private bool m_AutoSelectFirstPanel = true;

        private void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            for (int i = 0; i < m_Buttons.Count; ++i)
            {
                int nonVariableI = i;
                m_Buttons[i].onClick.AddListener(() => ShowPanel(nonVariableI));
            }

            int panelID = -1; //Activate all buttons & show no panels
            if (m_AutoSelectFirstPanel)
            {
                //Determine the first panel
                for (int i = 0; i < m_Buttons.Count; ++i)
                {
                    if (m_Buttons[i].IsActive())
                    {
                        panelID = i;
                        break;
                    }
                }
            }

            ShowPanel(panelID);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_Buttons.Count; ++i)
            {
                m_Buttons[i].onClick.RemoveAllListeners();
            }
        }

        public void ShowPanel(int id)
        {
            //if (id < 0 || id >= m_Panels.Count)
            //return;

            //Deactivatve the button
            for (int i = 0; i < m_Buttons.Count; ++i)
            {
                if (i == id)
                    m_Buttons[i].interactable = false;
                else
                    m_Buttons[i].interactable = true;
            }

            //Show the panel
            for (int i = 0; i < m_Panels.Count; ++i)
            {
                if (i == id)
                    m_Panels[i].SetActive(true);
                else
                    m_Panels[i].SetActive(false);
            }
        }

        private void OnActiveSceneChanged(Scene prevScene, Scene currentScene)
        {
            //When changing scenes close everything
            ShowPanel(-1);
        }
    }
}