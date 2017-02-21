using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSwitchPanel : MonoBehaviour
{
    [SerializeField]
    private List<Button> m_Buttons;

    [SerializeField]
    private List<GameObject> m_Panels;

    [SerializeField]
    private bool m_AutoSelectFirstPanel;

    private void Start()
    {
        for (int i = 0; i < m_Buttons.Count; ++i)
        {
            int nonVariableI = i;
            m_Buttons[i].onClick.AddListener(() => ShowPanel(nonVariableI));
        }

        if (m_AutoSelectFirstPanel)
            ShowPanel(0);
        else
            ShowPanel(-1); //Activate all buttons & show no panels
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
}
