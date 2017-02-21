using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    public delegate void FadeDelegate();

    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private float m_FadeSpeed;

    [SerializeField]
    private float m_MinAlpha;

    [SerializeField]
    private float m_MaxAlpha;
    private Coroutine m_FadeRoutine;


    public void SetAlpha(float alpha)
    {
        Color newColor = m_Image.color;
        newColor.a = alpha;
        m_Image.color = newColor;
    }

    public void SetAlphaMin()
    {
        SetAlpha(m_MinAlpha);
    }

    public void SetAlphaMax()
    {
        SetAlpha(m_MaxAlpha);
    }


    public void FadeOut()
    {
        FadeOut(null);
    }

    public void FadeOut(FadeDelegate callback)
    {
        StartFading(callback, m_MinAlpha);
    }

    public void FadeIn()
    {
        FadeIn(null);
    }

    public void FadeIn(FadeDelegate callback)
    {
        StartFading(callback, m_MaxAlpha);
    }


    private void StartFading(FadeDelegate callback, float targetAlhpa)
    {
        if (m_FadeRoutine != null)
            StopCoroutine(m_FadeRoutine);

        m_FadeRoutine = StartCoroutine(FadeRoutine(callback, targetAlhpa));
    }

    private IEnumerator FadeRoutine(FadeDelegate callback, float targetAlhpa)
    {
        while (m_Image.color.a != targetAlhpa)
        {
            float prevSign = Mathf.Sign(targetAlhpa - m_Image.color.a);

            //Change the color
            Color newColor = m_Image.color;
            newColor.a += prevSign * m_FadeSpeed * Time.fixedDeltaTime; //Ignores deltaTime

            float afterSign = Mathf.Sign(targetAlhpa - newColor.a);

            //We flipped
            if (prevSign != afterSign)
            {
                newColor.a = targetAlhpa;
            }

            m_Image.color = newColor;
            yield return new WaitForEndOfFrame();
        }

        if (callback != null)
            callback();

        m_FadeRoutine = null;
    }
}
