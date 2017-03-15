using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameSelectToggle : MonoBehaviour
{
    public delegate void SaveGameSelectDelegate(SaveGame definition);

    [SerializeField]
    private Text m_SaveGameName;

    [SerializeField]
    private Text m_LevelName;

    [SerializeField]
    private Text m_SaveTime;

    [SerializeField]
    private Text m_PlayTime;

    [SerializeField]
    private Image m_Picture;

    [SerializeField]
    private Toggle m_Toggle;
    private SaveGame m_SaveGame;
    public SaveGame SaveGame
    {
        get { return m_SaveGame; }
    }

    public event SaveGameSelectDelegate SaveGameSelectEvent;

    public void Setup(SaveGame saveGame, RectTransform parent, ToggleGroup toggleGroup)
    {
        m_SaveGame = saveGame;

        //Name
        if (m_SaveGame == null)
        {
            m_SaveGameName.text = "New save";
        }
        else
        {
            m_SaveGameName.text = m_SaveGame.Name;
        }

        //Timestamp
        DateTime timeStamp = DateTime.Now;
        if (m_SaveGame != null)
        {
            timeStamp = m_SaveGame.TimeStamp;
        }

        m_SaveTime.text = timeStamp.ToShortDateString() + " " +
                          timeStamp.Hour.ToString() + ":" +
                          timeStamp.Minute.ToString() + ":" +
                          timeStamp.Second.ToString();

        //Time played
        ulong remainingTimePlayed = 0;

        if (m_SaveGame == null)
        {
            remainingTimePlayed = SaveGameManager.Instance.ActiveSaveGame.PlayTime;
        }
        else
        {
            remainingTimePlayed = m_SaveGame.PlayTime;
        }

        ulong hoursPlayed = (remainingTimePlayed / 3600000);
        remainingTimePlayed -= hoursPlayed * 3600000;

        ulong minutesPlayed = (remainingTimePlayed / 60000); //Doesn't really need to be an ulong, but otherwise we have to cast around a lot
        remainingTimePlayed -= (minutesPlayed * 60000);

        if (hoursPlayed > 1) { m_PlayTime.text = hoursPlayed.ToString() + " hours played"; }
        if (hoursPlayed == 1) { m_PlayTime.text = hoursPlayed.ToString() + " hour played"; }

        if (hoursPlayed == 0)
        {
            if (minutesPlayed > 1) { m_PlayTime.text = minutesPlayed.ToString() + " minutes played"; }
            else                   { m_PlayTime.text = minutesPlayed.ToString() + " minute played"; }
        }

        //Level name & picture
        LevelDataDefinition levelData = null;
        if (m_SaveGame == null)
        {
            levelData = LevelManager.Instance.GetCurrentLevelData();
        }
        else
        {
            levelData = LevelManager.Instance.GetLevelData(m_SaveGame.LevelID);
        }

        m_LevelName.text = levelData.LevelName;
        m_Picture.sprite = levelData.Picture;


        transform.SetParent(parent);
        transform.SetSiblingIndex(0); //At the top of the list

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //Scale always goes nuts after parenting. Fix that.

        m_Toggle.group = toggleGroup;

        m_Toggle.onValueChanged.RemoveAllListeners();
        m_Toggle.onValueChanged.AddListener(OnToggleValueChange);
    }

    private void OnDestroy()
    {
        m_Toggle.onValueChanged.RemoveAllListeners();
    }

    public void OnToggleValueChange(bool value)
    {
        if (value == true)
        {
            if (SaveGameSelectEvent != null)
                SaveGameSelectEvent(m_SaveGame);
        }
    }

    public void IsOn(bool value)
    {
        m_Toggle.isOn = value;
    }
}
