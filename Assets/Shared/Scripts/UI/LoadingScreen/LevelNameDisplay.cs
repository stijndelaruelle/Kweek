using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNameDisplay : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        LevelDataDefinition levelData = LevelManager.Instance.GetCurrentLevelData();

        if (levelData == null)
            return;

        m_Text.text = levelData.LevelName;
    }
}
