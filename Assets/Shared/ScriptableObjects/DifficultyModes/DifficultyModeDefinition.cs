using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyModeDefinition : ScriptableObject
{
    [SerializeField]
    private string m_DifficultyName;
    public string DifficultyName
    {
        get { return m_DifficultyName; }
    }

    //More options are expected to follow.
    //Could become more than difficulty -> game mode
}