using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class LevelSelectPanel : MonoBehaviour
    {
        [SerializeField]
        private LevelSelectToggle m_TogglePrefab = null;

        [SerializeField]
        private RectTransform m_ContentRoot = null;

        [SerializeField]
        private ToggleGroup m_ToggleGroup = null;
        private List<LevelSelectToggle> m_LevelSelectToggles = null; //Sole purpose is to unsubscribe.

        private LevelDataDefinition m_SelectedLevelData = null;
        public LevelDataDefinition SelectedLevelData
        {
            get { return m_SelectedLevelData; }
        }

        private void Start()
        {
            //Create new toggles
            m_LevelSelectToggles = new List<LevelSelectToggle>();

            List<LevelDataDefinition> levelData = LevelManager.Instance.GetLevelDataList();
            for (int i = 0; i < levelData.Count; ++i)
            {
                if (levelData[i] != null)
                {
                    LevelSelectToggle toggle = GameObject.Instantiate<LevelSelectToggle>(m_TogglePrefab);
                    toggle.Setup(levelData[i], m_ContentRoot, m_ToggleGroup);
                    toggle.LevelSelectEvent += OnLevelSelect;

                    //Enable the first toggle
                    if (i == 0)
                        toggle.IsOn(true);
                    else
                        toggle.IsOn(false);

                    m_LevelSelectToggles.Add(toggle);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (LevelSelectToggle toggle in m_LevelSelectToggles)
            {
                toggle.LevelSelectEvent -= OnLevelSelect;
            }
        }

        private void OnLevelSelect(LevelDataDefinition levelData)
        {
            m_SelectedLevelData = levelData;
        }
    }
}