using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Kweek
{
    public class VolumeManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer m_Mixer = null;

        [SerializeField]
        private List<string> m_OptionVariables = null;

        private void Start()
        {
            OptionsManager.Instance.OptionChangedEvent += OnOptionChanged;
        }

        private void OnDestroy()
        {
            if (OptionsManager.Instance != null)
                OptionsManager.Instance.OptionChangedEvent -= OnOptionChanged;
        }

        private void SetVolume(string optionName, float value)
        {
            float normValue = value / 100.0f;

            float db = -80.0f;
            if (normValue > 0)
                db = (Mathf.Log10(normValue) * 20.0f) * 2;

            m_Mixer.SetFloat(optionName, db);
        }

        //Events
        private void OnOptionChanged(string key)
        {
            foreach (string optionName in m_OptionVariables)
            {
                if (optionName == key)
                {
                    SetVolume(optionName, OptionsManager.Instance.GetOptionAsFloat(optionName));
                }
            }
        }
    }
}