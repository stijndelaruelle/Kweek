using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class LoadingProgressText : MonoBehaviour
    {
        [SerializeField]
        private Text m_Text = null;

        [SerializeField]
        private SceneLoader m_SceneLoader = null;

        private void Update()
        {
            float percent = Mathf.Ceil(m_SceneLoader.GetProgress() * 100.0f);
            m_Text.text = percent + "%";
        }
    }
}