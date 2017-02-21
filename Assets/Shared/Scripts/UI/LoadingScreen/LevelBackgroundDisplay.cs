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
        m_Image.sprite = LevelManager.Instance.GetCurrentLevelData().Picture;
    }
}
