using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Looks silly, but this makes sure students don't have to edit the main menu file.
public class LevelDataListDefinition : ScriptableObject
{
    [SerializeField]
    private List<LevelDataDefinition> m_Levels;
    public List<LevelDataDefinition> Levels
    {
        get { return m_Levels; }
    }

    public int GetLevelCount()
    {
        return m_Levels.Count;
    }

    public LevelDataDefinition GetLevel(int id)
    {
        if (id < 0 || id >= m_Levels.Count)
            return null;

        return m_Levels[id];
    }

    public int GetLevelID(LevelDataDefinition levelData)
    {
        return m_Levels.IndexOf(levelData);
    }
}
