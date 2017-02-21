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
}
