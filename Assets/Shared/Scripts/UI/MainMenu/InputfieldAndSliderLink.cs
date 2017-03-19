using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class InputfieldAndSliderLink : MonoBehaviour
{
    [SerializeField]
    private Slider m_Slider;

    [SerializeField]
    private InputField m_InputField;

    [SerializeField]
    private float m_MaxValue;
    public float MaxValue
    {
        get { return m_MaxValue; }
    }

    public event Action<float> ValueChangedEvent;

    private void Start()
    {
        m_Slider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        m_InputField.onEndEdit.AddListener(delegate { OnInputFieldChanged(); });

        //Set the slider max value
        m_InputField.characterLimit = (int)Math.Floor(Math.Log10(m_MaxValue) + 1);

        //SetValue(m_MaxValue);
    }

    private void OnDestroy()
    {
        m_Slider.onValueChanged.RemoveListener(delegate { OnSliderChanged(); });
        m_InputField.onValueChanged.RemoveListener(delegate { OnInputFieldChanged(); });
    }

    public void SetValue(float value)
    {
        value = Mathf.Clamp(value, 0.0f, m_MaxValue);

        m_Slider.value = value;
        m_InputField.text = value.ToString();

        if (ValueChangedEvent != null)
            ValueChangedEvent(value);
    }

    //Events
    private void OnSliderChanged()
    {
        //Visually we count till 100
        SetValue(m_Slider.value);
    }

    private void OnInputFieldChanged()
    {
        float value = 0;
        bool success = float.TryParse(m_InputField.text, out value);

        if (success)
        {
            SetValue(value);
        }
    }
}
