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
        m_InputObject.ValueChangedEvent += OnValueChangedEvent;
    }

    private void OnDestroy()
    {
        if (m_InputObject != null)
            m_InputObject.ValueChangedEvent -= OnValueChangedEvent;
    }

    private void SetVolume(float value)
    {
        //Scale to -80 -> 0 //I don't like the idea of going to +20db
        float volume = (1.0f - (value / m_InputObject.MaxValue)) * -80.0f;
        m_Mixer.SetFloat(m_VariableName, volume);
    }

    //Events
    private void OnValueChangedEvent(float value)
    {
        SetVolume(value);
    }
}
