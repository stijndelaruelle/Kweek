using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

//Data container
public class SaveGame
{
    private string m_SaveGameName = "";     //TODO: Use this & add editing functionality
    public string Name
    {
        get { return m_SaveGameName; }
        set { m_SaveGameName = value; }
    }

    private int m_Difficulty = 0;
    public int Difficulty
    {
        get { return m_Difficulty; }
        set { m_Difficulty = value; }
    }

    private int m_LevelID = 0;
    public int LevelID
    {
        get { return m_LevelID; }
        set { m_LevelID = value; }
    }

    private DateTime m_Timestamp = DateTime.Now;
    public DateTime TimeStamp
    {
        get { return m_Timestamp; }
    }

    private ulong m_PlayTime = 0;
    public ulong PlayTime
    {
        get { return m_PlayTime; }
        set { m_PlayTime = value; }
    }

    private DirectoryInfo m_DirectoryInfo = null;
    public DirectoryInfo DirectoryInfo
    {
        get { return m_DirectoryInfo; }
        set { m_DirectoryInfo = value; }
    }

    //Not FileInfo, as this is always relative to the directory info
    private string m_MetaDataFileName = "";
    private string m_SaveGameFileName = "";

    public SaveGame()
    {
        m_SaveGameName = "Unnamed save file";
        m_LevelID = -1;
        m_PlayTime = 0;
    }

    public SaveGame(DirectoryInfo directoryInfo, string metaDataFileName, string saveGameFileName)
    {
        m_DirectoryInfo = directoryInfo;
        m_MetaDataFileName = metaDataFileName;
        m_SaveGameFileName = saveGameFileName;
    }

    public bool SaveMetaDataToDisk()
    {
        try
        {
            JSONClass rootObject = new JSONClass();
            Serialize(rootObject);

            //Write the JSON data (.ToString in release as it saves a lot of data compard to ToJSON)
            string jsonStr = "";
            #if !UNITY_EDITOR
                jsonStr = rootObject.ToString();
            #else
                jsonStr = rootObject.ToJSON(0);
            #endif

            File.WriteAllText(m_DirectoryInfo.FullName + "/" + m_MetaDataFileName, jsonStr);
        }
        catch (Exception e)
        {
            //The file was probably not found!
            UnityEngine.Debug.LogWarning("Error in SaveMetaDataToDisk: " + e.Message);
            return false;
        }

        return true;
    }

    public bool LoadMetaDataFromDisk()
    {
        string fileText = "";
        try
        {
            fileText = File.ReadAllText(m_DirectoryInfo.FullName + "/" + m_MetaDataFileName);
        }
        catch (Exception e)
        {
            //The file was probably not found!
            UnityEngine.Debug.LogWarning("Error in LoadMetaDataFromDisk: " + e.Message);
            return false;
        }

        try
        {
            JSONNode rootNode = JSON.Parse(fileText);
            Deserialize(rootNode);
        }
        catch (Exception e)
        {
            //There were errors parsing the file!
            UnityEngine.Debug.LogWarning("Error in LoadMetaDataFromDisk: " + e.Message);
            return false;
        }

        return true;
    }

    private void Serialize(JSONClass rootNode)
    {
        //Name
        JSONData nameData = new JSONData(m_SaveGameName);
        rootNode.Add("name", nameData);

        //Difficulty Mode
        JSONData difficultyData = new JSONData(m_Difficulty);
        rootNode.Add("difficulty", difficultyData);

        //Level ID
        JSONData levelIDData = new JSONData(m_LevelID);
        rootNode.Add("levelid", levelIDData);

        //Playtime
        JSONData playTimeData = new JSONData(m_PlayTime);
        rootNode.Add("playtime", playTimeData);

        //Timestamp
        JSONNode timeStampNode = new JSONData(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        rootNode.Add("timestamp", timeStampNode);
    }

    private void Deserialize(JSONNode jsonNode)
    {
        //Not in constructor as this can throw exceptions

        //Parse the name
        JSONNode nameNode = jsonNode["name"];
        if (nameNode == null) { throw new System.Exception("A save game doesn't contain a \"name\" node! Source: " + jsonNode.ToString()); }
        m_SaveGameName = nameNode.Value;

        //Parse the difficulty
        JSONNode difficultyNode = jsonNode["difficulty"];
        if (difficultyNode == null) { throw new System.Exception("A save game doesn't contain a \"difficulty\" node! Source: " + jsonNode.ToString()); }

        int difficulty;
        bool success = int.TryParse(difficultyNode.Value, out difficulty);
        if (success == false) { throw new System.Exception("A save game has an invalid \"difficulty\" node! Expected an integer. Source: " + jsonNode.ToString()); }

        m_Difficulty = difficulty;

        //Parse the level ID
        JSONNode levelIDNode = jsonNode["levelid"];
        if (levelIDNode == null) { throw new System.Exception("A save game doesn't contain a \"levelid\" node! Source: " + jsonNode.ToString()); }

        int levelID;
        success = int.TryParse(levelIDNode.Value, out levelID);
        if (success == false) { throw new System.Exception("A save game has an invalid \"levelid\" node! Expected an integer. Source: " + jsonNode.ToString()); }

        m_LevelID = levelID;

        //Parse the play time
        JSONNode playTimeNode = jsonNode["playtime"];
        if (playTimeNode == null) { throw new System.Exception("A save game doesn't contain a \"playtime\" node! Source: " + jsonNode.ToString()); }

        ulong playTime;
        success = ulong.TryParse(playTimeNode.Value, out playTime);
        if (success == false) { throw new System.Exception("A save game has an invalid \"playtime\" node! Expected an unsigned long. Source: " + jsonNode.ToString()); }

        m_PlayTime = playTime;
        
        //Parse the save time
        JSONNode timeStampNode = jsonNode["timestamp"];
        if (timeStampNode == null) { throw new System.Exception("A save game doesn't contain a \"savetime\" node! Source: " + jsonNode.ToString()); }
        m_Timestamp = DateTime.ParseExact(timeStampNode.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    //For students, use this function to get the filepath you want to parse
    public string GetSaveFilePath()
    {
        return m_DirectoryInfo.FullName + "/" + m_SaveGameFileName;
    }
}

public class SaveGameManager : Singleton<SaveGameManager>
{
    public delegate void SaveGameDelegate(SaveGame saveGame);
    public delegate void SaveGamesLoadedDelegate();

    [Tooltip("Folder in the \"My Games\" or \"Saved Games\" folder in the persons \"My Documents\"")]
    [SerializeField]
    private string m_SaveGameFolder;
    public string SaveGameFolder
    {
        get { return m_SaveGameFolder; }
    }

    private DirectoryInfo m_RootDirectory;

    [SerializeField]
    private string m_SaveGameDirectoryPrefix;

    [SerializeField]
    private string m_MetaDataFileName;

    [SerializeField]
    private string m_SaveGameFileName;

    private List<SaveGame> m_SaveGames;
    public List<SaveGame> SaveGames
    {
        get { return m_SaveGames; }
    }

    private SaveGame m_ActiveSaveGame;
    public SaveGame ActiveSaveGame
    {
        get { return m_ActiveSaveGame; }
    }

    //Events
    public event SaveGameDelegate SaveGameActivatedEvent; //Students should subscribe to this event to find out when a user has saved.

    public event SaveGameDelegate SaveGameAddedEvent;
    public event SaveGameDelegate SaveGameEditedEvent;
    public event SaveGameDelegate SaveGameDeletedEvent;
    public event SaveGamesLoadedDelegate SaveGamesLoadedEvent;

    protected override void Awake()
    {
        base.Awake();

        DirectoryInfo rootRootDirectory = new DirectoryInfo(SaveGameLocation.getSaveGameDirectory());
        m_RootDirectory = ExtentionMethods.FindOrCreateDirectory(rootRootDirectory, m_SaveGameFolder);

        m_SaveGames = new List<SaveGame>();
    }

    private void Start()
    {
        LoadSaveGamesFromDisk();
    }

    public void LoadSaveGamesFromDisk()
    {
        m_SaveGames.Clear();

        foreach (DirectoryInfo directory in m_RootDirectory.GetDirectories())
        {
            if (directory.Name.StartsWith(m_SaveGameDirectoryPrefix))
            {
                SaveGame newSaveGame = new SaveGame(directory, m_MetaDataFileName, m_SaveGameFileName);
                bool success = newSaveGame.LoadMetaDataFromDisk();

                //Only add the save game to the list if it was loaded correctly
                if (success)
                    m_SaveGames.Add(newSaveGame);
            }
        }

        if (SaveGamesLoadedEvent != null)
            SaveGamesLoadedEvent();
    }

    public void ActivateSaveGame(SaveGame saveGame)
    {
        m_ActiveSaveGame = saveGame;
        //Start counting play time?

        if (SaveGameActivatedEvent != null)
            SaveGameActivatedEvent(saveGame);
    }

    public void DeactivateSaveGame()
    {
        m_ActiveSaveGame = null;
    }

    public SaveGame CreateSaveGame(string name, int difficulty, int levelID, ulong playTime)
    {
        //Create a new folder for this save game
        string uniqueID = Guid.NewGuid().ToString("N");

        string uniqueName = ExtentionMethods.FindUniqueDirectoryName(m_RootDirectory, m_SaveGameDirectoryPrefix + uniqueID);
        DirectoryInfo directory = ExtentionMethods.FindOrCreateDirectory(m_RootDirectory, uniqueName);

        SaveGame newSaveGame = new SaveGame(directory, m_MetaDataFileName, m_SaveGameFileName);
        newSaveGame.Name = name;
        newSaveGame.Difficulty = difficulty;
        newSaveGame.LevelID = levelID;
        newSaveGame.PlayTime = playTime;

        bool success = newSaveGame.SaveMetaDataToDisk();

        //Only add the save game to the list if it was created correctly
        if (success)
        {
            m_SaveGames.Add(newSaveGame);

            if (SaveGameAddedEvent != null)
                SaveGameAddedEvent(newSaveGame);

            return newSaveGame;
        }

        return null;
    }

    public void EditSaveGame(SaveGame saveGame, string name, int difficulty, int levelID, ulong playTime)
    {
        saveGame.Name = name;
        saveGame.Difficulty = difficulty;
        saveGame.LevelID = levelID;
        saveGame.PlayTime = playTime;

        bool success = saveGame.SaveMetaDataToDisk();

        //Only add the save game to the list if it was created correctly
        if (success)
        {
            if (SaveGameEditedEvent != null)
                SaveGameEditedEvent(saveGame);
        }
    }

    public void DeleteSaveGame(SaveGame saveGame)
    {
        //We won't 100% delete the save game until a user manually deletes it, as you can never know if this was an accident.
        //Let's move it to a "Deleted save games" folder instead.

        if (saveGame == null)
            return;

        try
        {
            DirectoryInfo deletedDirectory = ExtentionMethods.FindOrCreateDirectory(m_RootDirectory, "Deleted save games");

            string uniqueDirectoryName = ExtentionMethods.FindUniqueDirectoryName(deletedDirectory, saveGame.DirectoryInfo.Name);

            saveGame.DirectoryInfo.MoveTo(deletedDirectory.FullName + "/" + uniqueDirectoryName);

            m_SaveGames.Remove(saveGame);

            if (SaveGameDeletedEvent != null)
                SaveGameDeletedEvent(saveGame);
        }
        catch(Exception e)
        {
            //Possibilities: File in use, etc..
            UnityEngine.Debug.LogWarning("Error in DeleteSaveGame: " + e.Message);
        }
    }


    //Utility
    public void ActivateMostRecent()
    {
        List<SaveGame> sortedSaveGamesData = m_SaveGames.OrderByDescending(o => o.TimeStamp).ToList(); //Last saved game at the top of the list

        if (sortedSaveGamesData.Count > 0)
        {
            ActivateSaveGame(sortedSaveGamesData[0]);
        }
    }

    public int GetSaveGameCount()
    {
        return m_SaveGames.Count;
    }
}
