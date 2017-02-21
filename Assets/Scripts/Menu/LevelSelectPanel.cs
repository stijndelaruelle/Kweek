using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField]
    private LevelDataListDefinition m_LevelList;

    [SerializeField]
    private LevelSelectToggle m_TogglePrefab;

    [SerializeField]
    private RectTransform m_ContentRoot;

    [SerializeField]
    private ToggleGroup m_ToggleGroup;

    private void Start()
    {
        //Create new toggles
        List<LevelDataDefinition> levelData = m_LevelList.Levels;
        for (int i = 0; i < levelData.Count; ++i)
        {
            LevelSelectToggle toggle = GameObject.Instantiate<LevelSelectToggle>(m_TogglePrefab);
            toggle.Setup(levelData[i], m_ContentRoot, m_ToggleGroup);

            //Enable the first toggle
            if (i == 0)
                toggle.IsOn(true);
            else
                toggle.IsOn(false);
        }
    }
}
