using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class DifficultySelectPanel : MonoBehaviour
    {
        [SerializeField]
        private DifficultyModeListDefinition m_DifficultyModes;

        [SerializeField]
        private DifficultySelectToggle m_TogglePrefab;

        [SerializeField]
        private RectTransform m_ContentRoot;

        [SerializeField]
        private ToggleGroup m_ToggleGroup;
        private List<DifficultySelectToggle> m_DifficultySelectToggles; //Sole purpose is to unsubscribe.

        private int m_SelectedDifficulty = 0;
        public int SelectedDifficulty
        {
            get { return m_SelectedDifficulty; }
        }

        private void Start()
        {
            //Create new toggles
            m_DifficultySelectToggles = new List<DifficultySelectToggle>();

            for (int i = 0; i < m_DifficultyModes.GetDifficultyModeCount(); ++i)
            {
                DifficultySelectToggle toggle = GameObject.Instantiate<DifficultySelectToggle>(m_TogglePrefab);
                toggle.Setup(i, m_DifficultyModes.GetDifficultyMode(i), m_ContentRoot, m_ToggleGroup);
                toggle.DifficultySelectEvent += OnDifficultySelect;

                //Enable the first toggle
                if (i == 0)
                    toggle.IsOn(true);
                else
                    toggle.IsOn(false);

                m_DifficultySelectToggles.Add(toggle);
            }
        }

        private void OnDestroy()
        {
            if (m_DifficultySelectToggles == null)
                return;

            foreach (DifficultySelectToggle toggle in m_DifficultySelectToggles)
            {
                toggle.DifficultySelectEvent -= OnDifficultySelect;
            }
        }

        private void OnDifficultySelect(int difficulty)
        {
            m_SelectedDifficulty = difficulty;
        }
    }
}
