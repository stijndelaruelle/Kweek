﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public delegate void LevelManagerDelegate();

    [SerializeField]
    private LevelDataListDefinition m_LevelList;

    [SerializeField]
    private SceneLoader m_SceneLoader;

    [SerializeField]
    private string m_LoadingScreenSceneName;

    [SerializeField]
    private string m_MainMenuSceneName;

    private int m_CurrentLevelID = -1;

    //public event LevelManagerDelegate PreparedLevelLoadEvent;

    private void Update()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.F1))
        {
            LoadNextLevel();
        }
    }

    public void LoadMainMenu()
    {
        m_CurrentLevelID = -1;
        m_SceneLoader.LoadScene(m_MainMenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, false, true);
    }

    public void LoadLevel(int id)
    {
        m_CurrentLevelID = id;

        //if the ID is invalid, return to the main menu
        if (id < 0 || id >= m_LevelList.GetLevelCount() || GetCurrentLevelData() == null)
        {
            LoadMainMenu();
            return;
        }

        m_SceneLoader.LoadScene(m_LoadingScreenSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, false, true);
    }

    public void LoadLevel(LevelDataDefinition levelData)
    {
        if (levelData == null)
        {
            Debug.Log("Tried loading an invalid level!");
            LoadMainMenu();
            return;
        }

        LoadLevel(m_LevelList.GetLevelID(levelData));
    }


    public void LoadNextLevel()
    {
        m_CurrentLevelID += 1;
        LoadLevel(m_CurrentLevelID);
    }

    public int GetCurrentLevelID()
    {
        return m_CurrentLevelID;
    }

    public LevelDataDefinition GetCurrentLevelData()
    {
        return m_LevelList.GetLevel(m_CurrentLevelID);
    }

    public bool IsCurrentLevelLoaded()
    {
        if (m_CurrentLevelID < 0)
            return false;

        LevelDataDefinition levelData = m_LevelList.GetLevel(m_CurrentLevelID);
        if (levelData == null)
            return false;

        Scene currentScene = SceneManager.GetSceneByName(levelData.SceneName);
        Scene activeScene = SceneManager.GetActiveScene();

        return (activeScene == currentScene);
    }


    public List<LevelDataDefinition> GetLevelDataList()
    {
        return m_LevelList.Levels;
    }

    public LevelDataDefinition GetLevelData(int levelID)
    {
        return m_LevelList.GetLevel(levelID);
    }

    public int GetLevelID(LevelDataDefinition level)
    {
        return m_LevelList.GetLevelID(level);
    }
}
