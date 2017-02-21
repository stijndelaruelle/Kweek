using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectToggle : MonoBehaviour
{
    public delegate void LevelSelectDelegate(LevelDataDefinition definition);

    [SerializeField]
    private Text m_LevelName;

    [SerializeField]
    private Image m_Picture;

    [SerializeField]
    private Toggle m_Toggle;
    private LevelDataDefinition m_LevelData;

    public event LevelSelectDelegate LevelSelectEvent;

    public void Setup(LevelDataDefinition definition, RectTransform parent, ToggleGroup toggleGroup)
    {
        m_LevelData = definition;

        m_LevelName.text = definition.LevelName;
        m_Picture.sprite = definition.Picture;

        transform.SetParent(parent);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //Scale always goes nuts after parenting. Fix that.

        m_Toggle.group = toggleGroup;

        m_Toggle.onValueChanged.RemoveAllListeners();
        m_Toggle.onValueChanged.AddListener(OnToggleValueChange);
    }

    private void OnDestroy()
    {
        m_Toggle.onValueChanged.RemoveAllListeners();    
    }

    public void OnToggleValueChange(bool value)
    {
        if (value == true)
        {
            if (LevelSelectEvent != null)
                LevelSelectEvent(m_LevelData);
        }
    }

    public void IsOn(bool value)
    {
        m_Toggle.isOn = value;
    }
}
