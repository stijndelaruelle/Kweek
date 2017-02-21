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
        m_Text.text = LevelManager.Instance.GetCurrentLevelData().LevelName;
    }
}
