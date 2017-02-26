using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;

//Data container
public class SaveGame
{
    private string m_SaveGameName;
    public string Name
    {
        get { return m_SaveGameName; }
        set { m_SaveGameName = value; }
    }

    private int m_LevelID;
    public int LevelID
    {
        get { return m_LevelID; }
        set { m_LevelID = value; }
    }

    private DateTime m_SaveTime;
    public DateTime SaveTime
    {
        get { return m_SaveTime; }
        set { m_SaveTime = value; }
    }

    private ulong m_PlayTime;
    public ulong PlayTime
    {
        get { return m_PlayTime; }
        set { m_PlayTime = value; }
    }
    private Stopwatch m_StopWatch;

    private DirectoryInfo m_DirectoryInfo;
    public DirectoryInfo DirectoryInfo
    {
        get { return m_DirectoryInfo; }
        set { m_DirectoryInfo = value; }
    }

    //Not FileInfo, as this is always relative to the directory info
    private string m_MetaDataFileName;
    private string m_SaveGameFileName;

    public SaveGame()
    {
        m_StopWatch = new Stopwatch();

        m_SaveGameName = "Unnamed save file";
        m_LevelID = -1;
        m_PlayTime = 0;
    }

    public SaveGame(DirectoryInfo directoryInfo, string metaDataFileName, string saveGameFileName)
    {
        m_StopWatch = new Stopwatch();

        m_DirectoryInfo = directoryInfo;
        m_MetaDataFileName = metaDataFileName;
        m_SaveGameFileName = saveGameFileName;
    }

    public void SaveMetaDataToDisk()
    {

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

    private JSONNode Serialize()
    {
        JSONClass data = new JSONClass();

        //Location
        JSONData nameData = new JSONData(m_SaveGameName);
        data.Add("name", nameData);

        //Smilies
        JSONData levelIDData = new JSONData(m_LevelID);
        data.Add("levelid", levelIDData);

        //Last smiley fill
        JSONData playTimeData = new JSONData(m_PlayTime);
        data.Add("playtime", playTimeData);

        return data;
    }

    private void Deserialize(JSONNode jsonNode)
    {
        //Not in constructor as this can throw exceptions

        //Parse the name
        JSONNode nameNode = jsonNode["name"];
        if (nameNode == null) { throw new System.Exception("A save game doesn't contain a \"name\" node! Source: " + jsonNode.ToString()); }
        m_SaveGameName = nameNode.Value;


        //Parse the level ID
        JSONNode levelIDNode = jsonNode["levelid"];
        if (levelIDNode == null) { throw new System.Exception("A save game doesn't contain a \"levelid\" node! Source: " + jsonNode.ToString()); }

        int levelID;
        bool success = int.TryParse(levelIDNode.Value, out levelID);
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
        JSONNode saveTimeNode = jsonNode["savetime"];
        if (saveTimeNode == null) { throw new System.Exception("A save game doesn't contain a \"savetime\" node! Source: " + jsonNode.ToString()); }
        m_SaveTime = DateTime.ParseExact(saveTimeNode.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}

public class SaveGameManager : Singleton<SaveGameManager>
{
    public delegate void SaveGameDelegate(SaveGame saveGame);
    public delegate void SaveGamesLoadedDelegate();

    [SerializeField]
    private string m_RootPath;

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

    //Events
    public event SaveGameDelegate SaveGameAddedEvent;
    public event SaveGameDelegate SaveGameEditedEvent;
    public event SaveGameDelegate SaveGameDeletedEvent;
    public event SaveGamesLoadedDelegate SaveGamesLoadedEvent;

    protected override void Awake()
    {
        base.Awake();
        m_SaveGames = new List<SaveGame>();
    }

    public void LoadSaveGamesFromDisk()
    {
        m_SaveGames.Clear();

        DirectoryInfo rootDirectory = new DirectoryInfo(m_RootPath);

        foreach (DirectoryInfo directory in rootDirectory.GetDirectories())
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

    public void CreateSaveGame(string name)
    {

    }

    public void EditSaveGame(SaveGame saveGame, string name, int levelID, int blabla)
    {

    }

    public void DeleteSaveGame(SaveGame saveGame)
    {
        //We won't 100% delete the save game until a user manually deletes it, as you can never know if this was an accident.
        //Let's move it to a "Deleted save games" folder instead.

        try
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(m_RootPath);
            DirectoryInfo deletedDirectory = FindOrCreateDirectory(rootDirectory, "Deleted save games");

            string uniqueDirectoryName = FindUniqueDirectoryName(deletedDirectory, saveGame.DirectoryInfo.Name);

            saveGame.DirectoryInfo.MoveTo(deletedDirectory.FullName + "/" + uniqueDirectoryName);

            if (SaveGameDeletedEvent != null)
                SaveGameDeletedEvent(saveGame);
        }
        catch(Exception e)
        {
            //Possibilities: File in use, etc..
            UnityEngine.Debug.LogWarning("Error in DeleteSaveGame: " + e.Message);
        }
    }

    private DirectoryInfo FindOrCreateDirectory(DirectoryInfo rootDirectory, string name)
    {
        if (rootDirectory == null)
            return null;

        //Check if that folder already exists
        DirectoryInfo[] subDirectories = rootDirectory.GetDirectories();
        DirectoryInfo ourDirectory = null;

        foreach (DirectoryInfo directory in subDirectories)
        {
            if (directory.Name == name)
            {
                ourDirectory = directory;
                break;
            }
        }

        //If not, create it.
        if (ourDirectory == null)
        {
            ourDirectory = rootDirectory.CreateSubdirectory(name);
        }

        return ourDirectory;
    }

    private string FindUniqueDirectoryName(DirectoryInfo rootDirectory, string originalDirectoryName)
    {
        if (rootDirectory == null)
            return "";

        string uniqueFileName = originalDirectoryName;

        int count = 0;
        DirectoryInfo[] directories = rootDirectory.GetDirectories();
        for (int i = 0; i < directories.Length; ++i)
        {
            if (directories[i].Name.StartsWith(originalDirectoryName))
            {
                string testFilename = originalDirectoryName;
                if (count > 0) { testFilename += " (" + (count + 1) + ")"; }

                if (directories[i].Name != testFilename)
                {
                    uniqueFileName = testFilename;
                    break;
                }

                ++count;

                uniqueFileName = originalDirectoryName + " (" + (count + 1) + ")";
            }
        }

        return uniqueFileName;
    }
}
