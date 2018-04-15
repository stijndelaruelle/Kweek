using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBackgroundDisplay : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    private void Start()
    {
        LevelDataDefinition levelData = LevelManager.Instance.GetCurrentLevelData();

        if (levelData == null)
            return;

        Sprite sprite = levelData.Picture;

        if (sprite != null)
            m_Image.sprite = sprite;
    }
}
