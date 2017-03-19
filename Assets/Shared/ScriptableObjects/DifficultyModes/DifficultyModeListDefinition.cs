using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyModeListDefinition : ScriptableObject
{
    [SerializeField]
    private List<DifficultyModeDefinition> m_DifficultyModes;
    public List<DifficultyModeDefinition> DifficultyModes
    {
        get { return m_DifficultyModes; }
    }

    public int GetDifficultyModeCount()
    {
        return m_DifficultyModes.Count;
    }

    public DifficultyModeDefinition GetDifficultyMode(int id)
    {
        if (id < 0 || id >= m_DifficultyModes.Count)
            return null;

        return m_DifficultyModes[id];
    }

    public int GetDifficultyModeID(DifficultyModeDefinition difficultyMode)
    {
        return m_DifficultyModes.IndexOf(difficultyMode);
    }
}
