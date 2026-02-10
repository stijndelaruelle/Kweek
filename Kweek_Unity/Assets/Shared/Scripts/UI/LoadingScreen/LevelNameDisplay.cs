using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class LevelNameDisplay : MonoBehaviour
    {
        [SerializeField]
        private Text m_Text = null;

        private void Start()
        {
            LevelDataDefinition levelData = LevelManager.Instance.GetCurrentLevelData();

            if (levelData == null)
                return;

            m_Text.text = levelData.LevelName;
        }
    }
}