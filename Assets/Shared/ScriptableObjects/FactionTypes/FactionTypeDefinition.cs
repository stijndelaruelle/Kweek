using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "FactionTypeDefinition", menuName = "Kweek/Faction Type Definition")]
    public class FactionTypeDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_FactionName = string.Empty;

        [SerializeField]
        private List<FactionTypeDefinition> m_Allies = null;

        [SerializeField]
        private List<FactionTypeDefinition> m_Enemies = null;

        public bool IsAlly(FactionTypeDefinition factionType)
        {
            if (m_Allies == null)
                return false;

            return (m_Allies.Contains(factionType));
        }

        public bool IsEnemy(FactionTypeDefinition factionType)
        {
            if (m_Enemies == null)
                return false;

            return (m_Enemies.Contains(factionType));
        }

        public override string ToString()
        {
            return "Faction: " + m_FactionName;
        }
    }
}