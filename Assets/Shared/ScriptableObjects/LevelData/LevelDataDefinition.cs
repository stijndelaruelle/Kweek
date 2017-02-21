using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataDefinition : ScriptableObject
{
    [SerializeField]
    private string m_LevelName;
    public string LevelName
    {
        get { return m_LevelName; }
    }

    [SerializeField]
    private string m_SceneName;
    public string SceneName
    {
        get { return m_SceneName; }
    }

    [SerializeField]
    private Sprite m_Picture;
    public Sprite Picture
    {
        get { return m_Picture; }
    }
}