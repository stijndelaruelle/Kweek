using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "LevelDataDefinition", menuName = "Kweek/Level Data Definition")]
    public class LevelDataDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_LevelName = string.Empty;
        public string LevelName
        {
            get { return m_LevelName; }
        }

        [SerializeField]
        private string m_SceneName = string.Empty;
        public string SceneName
        {
            get { return m_SceneName; }
        }

        [SerializeField]
        private Sprite m_Picture = null;
        public Sprite Picture
        {
            get { return m_Picture; }
        }
    }
}