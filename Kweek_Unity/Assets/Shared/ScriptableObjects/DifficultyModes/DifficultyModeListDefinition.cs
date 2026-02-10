using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "DifficultyModeListDefinition", menuName = "Kweek/Difficulty Mode List Definition")]
    public class DifficultyModeListDefinition : ScriptableObject
    {
        [SerializeField]
        private List<DifficultyModeDefinition> m_DifficultyModes = null;
        public List<DifficultyModeDefinition> DifficultyModes
        {
            get { return m_DifficultyModes; }
        }

        public int GetDifficultyModeCount()
        {
            if (m_DifficultyModes == null)
                return 0;

            return m_DifficultyModes.Count;
        }

        public DifficultyModeDefinition GetDifficultyMode(int id)
        {
            if (m_DifficultyModes == null)
                return null;

            if (id < 0 || id >= m_DifficultyModes.Count)
                return null;

            return m_DifficultyModes[id];
        }

        public int GetDifficultyModeID(DifficultyModeDefinition difficultyMode)
        {
            if (m_DifficultyModes == null)
                return -1;

            return m_DifficultyModes.IndexOf(difficultyMode);
        }
    }
}