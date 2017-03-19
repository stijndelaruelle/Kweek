using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionTypeDefinition : ScriptableObject
{
    [SerializeField]
    private string m_FactionName;

    [SerializeField]
    private List<FactionTypeDefinition> m_Allies;

    [SerializeField]
    private List<FactionTypeDefinition> m_Enemies;

    public bool IsAlly(FactionTypeDefinition factionType)
    {
        return (m_Allies.Contains(factionType));
    }

    public bool IsEnemy(FactionTypeDefinition factionType)
    {
        return (m_Enemies.Contains(factionType));
    }

    public override string ToString()
    {
        return "Faction: " + m_FactionName;
    }
}
