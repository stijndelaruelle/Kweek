using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectToggle : MonoBehaviour
{
    [SerializeField]
    private Text m_LevelName;

    [SerializeField]
    private Image m_Picture;

    [SerializeField]
    private Toggle m_Toggle;
    
    public void Setup(LevelDataDefinition definition, RectTransform parent, ToggleGroup toggleGroup)
    {
        m_LevelName.text = definition.Name;
        m_Picture.sprite = definition.Picture;

        transform.SetParent(parent);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //Scale always goes nuts after parenting. Fix that.

        m_Toggle.group = toggleGroup;
    }

    public void IsOn(bool value)
    {
        m_Toggle.isOn = value;
    }
}
