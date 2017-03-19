using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private AudioMixer m_Mixer;

    [SerializeField]
    private string m_VariableName;

    [SerializeField]
    private InputfieldAndSliderLink m_InputObject;

    private void Start()
    {
        m_InputObject.ValueChangedEvent += OnValueChanged;
    }

    private void OnDestroy()
    {
        if (m_InputObject != null)
            m_InputObject.ValueChangedEvent -= OnValueChanged;
    }

   
    private void SetVolume(float value)
    {
        float normValue = value / m_InputObject.MaxValue;

        float t = Mathf.Log10(normValue * 20.0f);
        float volume = Mathf.Lerp(-80f, 0f, t); //I don't like the idea of going to +20db
        m_Mixer.SetFloat(m_VariableName, volume);
    }

    //Events
    private void OnValueChanged(float value)
    {
        SetVolume(value);
    }
}
