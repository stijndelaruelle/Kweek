using UnityEngine;

namespace Kweek
{
    public class FactionType : MonoBehaviour
    {
        [SerializeField]
        private FactionTypeDefinition m_FactionType = null;
        public FactionTypeDefinition Faction
        {
            get { return m_FactionType; }
        }


        public bool IsAlly(FactionType factionType)
        {
            return m_FactionType.IsAlly(factionType.Faction);
        }

        public bool IsAlly(FactionTypeDefinition factionType)
        {
            return m_FactionType.IsAlly(factionType);
        }

        public bool IsEnemy(FactionType factionType)
        {
            return m_FactionType.IsEnemy(factionType.Faction);
        }

        public bool IsEnemy(FactionTypeDefinition factionType)
        {
            return m_FactionType.IsEnemy(factionType);
        }


        public override string ToString()
        {
            return "Faction: " + m_FactionType.ToString();
        }
    }
}