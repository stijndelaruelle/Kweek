using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class DifficultySelectToggle : MonoBehaviour
    {
        public delegate void DifficultySelectDelegate(int difficulty);

        [SerializeField]
        private Text m_DifficultyName = null;

        [SerializeField]
        private Toggle m_Toggle = null;

        private int m_DifficultyID = 0;
        private DifficultyModeDefinition m_DifficultyMode = null;

        //Events
        public DifficultySelectDelegate DifficultySelectEvent = null;

        public void Setup(int difficultyID, DifficultyModeDefinition difficultyMode, RectTransform parent, ToggleGroup toggleGroup)
        {
            m_DifficultyID = difficultyID;
            m_DifficultyMode = difficultyMode;

            m_DifficultyName.text = m_DifficultyMode.DifficultyName;

            transform.SetParent(parent);
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //Scale always goes nuts after parenting. Fix that.

            m_Toggle.group = toggleGroup;

            m_Toggle.onValueChanged.RemoveAllListeners();
            m_Toggle.onValueChanged.AddListener(OnToggleValueChange);
        }

        private void OnDestroy()
        {
            if (m_Toggle != null)
                m_Toggle.onValueChanged.RemoveAllListeners();
        }

        public void OnToggleValueChange(bool value)
        {
            if (value == true)
            {
                if (DifficultySelectEvent != null)
                    DifficultySelectEvent(m_DifficultyID);
            }
        }

        public void IsOn(bool value)
        {
            m_Toggle.isOn = value;
        }
    }
}