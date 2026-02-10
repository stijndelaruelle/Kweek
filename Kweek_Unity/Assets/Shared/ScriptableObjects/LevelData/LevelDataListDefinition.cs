using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "LevelDataListDefinition", menuName = "Kweek/Level Data List Definition")]
    public class LevelDataListDefinition : ScriptableObject
    {
        [SerializeField]
        private List<LevelDataDefinition> m_Levels = null;
        public List<LevelDataDefinition> Levels
        {
            get { return m_Levels; }
        }

        public int GetLevelCount()
        {
            if (m_Levels == null)
                return 0;

            return m_Levels.Count;
        }

        public LevelDataDefinition GetLevel(int id)
        {
            if (m_Levels == null)
                return null;

            if (id < 0 || id >= m_Levels.Count)
                return null;

            return m_Levels[id];
        }

        public int GetLevelID(LevelDataDefinition levelData)
        {
            if (m_Levels == null)
                return -1;

            return m_Levels.IndexOf(levelData);
        }
    }
}