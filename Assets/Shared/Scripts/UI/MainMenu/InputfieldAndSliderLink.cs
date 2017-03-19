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
    private float m_DefaultValue;

    [SerializeField]
    private float m_MaxValue;
    public float MaxValue
    {
        get { return m_MaxValue; }
    }

    [SerializeField]
    private int m_MaxDecimalNumbers;

    [SerializeField]
    private string m_OptionVariable;

    public event Action<float> ValueChangedEvent;

    private void Start()
    {
        m_Slider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        m_InputField.onEndEdit.AddListener(delegate { OnInputFieldChanged(); });

        //Set the slider max value
        m_Slider.maxValue = m_MaxValue;
        m_Slider.wholeNumbers = (m_MaxDecimalNumbers == 0);

        int characterLimit = (int)Math.Floor(Math.Log10(m_MaxValue) + 1);
        if (m_MaxDecimalNumbers > 0)
            characterLimit += m_MaxDecimalNumbers + 1; //1 = the decimal .

        m_InputField.characterLimit = characterLimit;
    }

    private void OnDestroy()
    {
        m_Slider.onValueChanged.RemoveListener(delegate { OnSliderChanged(); });
        m_InputField.onValueChanged.RemoveListener(delegate { OnInputFieldChanged(); });
    }

    private void OnEnable()
    {
        if (OptionsManager.Instance != null && m_OptionVariable != "")
        {
            float value = m_DefaultValue;
            bool success = OptionsManager.Instance.GetOptionAsFloat(m_OptionVariable, out value);

            if (success)
                SetValue(value);
            else
                SetValue(m_DefaultValue);
        }
    }

    private void SetValue(float value)
    {
        value = Mathf.Clamp(value, 0.0f, m_MaxValue);

        if (m_Slider.value == value && m_InputField.text == value.ToString())
            return;

        m_Slider.value = value;

        string format = "0";
        if (m_MaxDecimalNumbers > 0) { format += "."; }
        for (int i = 0; i < m_MaxDecimalNumbers; ++i) { format += "0"; }

        m_InputField.text = value.ToString(format);

        if (ValueChangedEvent != null)
            ValueChangedEvent(value);
    }

    private void SaveOption(float value)
    {
        OptionsManager.Instance.SetOption(m_OptionVariable, value);
    }

    //Events
    private void OnSliderChanged()
    {
        SetValue(m_Slider.value);
        SaveOption(m_Slider.value);
    }

    private void OnInputFieldChanged()
    {
        float value = 0.0f;
        bool success = float.TryParse(m_InputField.text, out value);

        if (success)
        {
            SetValue(value);
            SaveOption(value);
        }
    }
}
