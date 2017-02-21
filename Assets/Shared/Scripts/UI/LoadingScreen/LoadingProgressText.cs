using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressText : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private SceneLoader m_SceneLoader;

    private void Update()
    {
        float percent = Mathf.Ceil(m_SceneLoader.GetProgress() * 100.0f);
        m_Text.text = percent + "%";
    }

}
