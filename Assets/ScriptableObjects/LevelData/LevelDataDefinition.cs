using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataDefinition : ScriptableObject
{
    [SerializeField]
    private string m_Name;
    public string Name
    {
        get { return m_Name; }
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