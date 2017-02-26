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
        m_SaveGameName.text = m_SaveGame.Name;

        //Saved Time
        m_SaveTime.text = m_SaveGame.SaveTime.ToShortDateString() + " " +
                          m_SaveGame.SaveTime.Hour.ToString() + ":" +
                          m_SaveGame.SaveTime.Minute.ToString() + ":" +
                          m_SaveGame.SaveTime.Second.ToString();

        //Time played
        ulong remainingTimePlayed = m_SaveGame.PlayTime;

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
        LevelDataDefinition levelData = LevelManager.Instance.GetLevelData(saveGame.LevelID);
        m_LevelName.text = levelData.LevelName;
        m_Picture.sprite = levelData.Picture;

        transform.SetParent(parent);
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
