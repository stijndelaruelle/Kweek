using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "DifficultyModeDefinition", menuName = "Kweek/Difficulty Mode Definition")]
    public class DifficultyModeDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_DifficultyName = string.Empty;
        public string DifficultyName
        {
            get { return m_DifficultyName; }
        }

        //More options are expected to follow.
        //Could become more than difficulty -> game mode
    }
}