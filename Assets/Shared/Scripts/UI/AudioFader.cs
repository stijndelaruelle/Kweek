using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioFader : MonoBehaviour
{
    public delegate void FadeDelegate();

    [SerializeField]
    private AudioMixer m_Mixer;

    [SerializeField]
    private string m_VariableName;

    [SerializeField]
    private float m_FadeSpeed;

    [SerializeField]
    private float m_MinVolume;

    [SerializeField]
    private float m_MaxVolume;
    private Coroutine m_FadeRoutine;

    private float GetVolume()
    {
        float volume = 0.0f;
        bool success = m_Mixer.GetFloat(m_VariableName, out volume);

        if (success == false)
            Debug.LogError("AudioFader is trying to change the volume of " + m_Mixer.ToString() + ": " + m_VariableName);

        return volume;
    }

    public void SetVolume(float volume)
    {
        bool success = m_Mixer.SetFloat(m_VariableName, volume);

        if (success == false)
            Debug.LogError("AudioFader is trying to change the volume of " + m_Mixer.ToString() + ": " + m_VariableName);
    }

    public void SetVolumeMin()
    {
        SetVolume(m_MinVolume);
    }

    public void SetVolumeMax()
    {
        SetVolume(m_MaxVolume);
    }


    public void FadeOut()
    {
        FadeOut(null);
    }

    public void FadeOut(FadeDelegate callback)
    {
        StartFading(callback, m_MinVolume);
    }

    public void FadeIn()
    {
        FadeIn(null);
    }

    public void FadeIn(FadeDelegate callback)
    {
        StartFading(callback, m_MaxVolume);
    }


    private void StartFading(FadeDelegate callback, float targetAlhpa)
    {
        if (m_FadeRoutine != null)
            StopCoroutine(m_FadeRoutine);

        m_FadeRoutine = StartCoroutine(FadeRoutine(callback, targetAlhpa));
    }

    private IEnumerator FadeRoutine(FadeDelegate callback, float targetAlhpa)
    {
        float currentVolume = GetVolume();
        while (currentVolume != targetAlhpa)
        {
            float prevSign = Mathf.Sign(targetAlhpa - currentVolume);

            //Change the volume
            float newVolume = currentVolume + (prevSign * m_FadeSpeed * Time.deltaTime);

            float afterSign = Mathf.Sign(targetAlhpa - newVolume);

            //We flipped
            if (prevSign != afterSign)
            {
                newVolume = targetAlhpa;
            }

            SetVolume(newVolume);
            currentVolume = newVolume;

            yield return new WaitForEndOfFrame();
        }

        if (callback != null)
            callback();

        m_FadeRoutine = null;
    }
}
