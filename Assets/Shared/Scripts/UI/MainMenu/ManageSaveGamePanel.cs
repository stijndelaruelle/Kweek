using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageSaveGamePanel : MonoBehaviour
{
    [SerializeField]
    private SaveGameSelectToggle m_TogglePrefab;

    [SerializeField]
    private RectTransform m_ContentRoot;

    [SerializeField]
    private ToggleGroup m_ToggleGroup;
    private List<SaveGameSelectToggle> m_SaveGameSelectToggles;

    [SerializeField]
    private PopupPanel m_PopupWindow;

    private SaveGame m_SelectedSaveGame = null;
    public SaveGame SelectedSaveGame
    {
        get { return m_SelectedSaveGame; }
    }

    private void Start()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;

        saveGameManager.SaveGameAddedEvent += OnSaveGameAdded;
        saveGameManager.SaveGameEditedEvent += OnSaveGameEdited;
        saveGameManager.SaveGameDeletedEvent += OnSaveGameDeleted;
        saveGameManager.SaveGamesLoadedEvent += OnSaveGamesLoaded;

        m_SaveGameSelectToggles = new List<SaveGameSelectToggle>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;
        if (saveGameManager != null)
        {
            saveGameManager.SaveGameAddedEvent -= OnSaveGameAdded;
            saveGameManager.SaveGameEditedEvent -= OnSaveGameEdited;
            saveGameManager.SaveGameDeletedEvent -= OnSaveGameDeleted;
            saveGameManager.SaveGamesLoadedEvent -= OnSaveGamesLoaded;
        }

        foreach (SaveGameSelectToggle toggle in m_SaveGameSelectToggles)
        {
            toggle.SaveGameSelectEvent -= OnSaveGameSelect;
        }
    }

    private void Refresh()
    {
        if (SaveGameManager.Instance != null)
            SaveGameManager.Instance.LoadSaveGamesFromDisk();
    }


    private void OnSaveGameSelect(SaveGame saveGame)
    {
        m_SelectedSaveGame = saveGame;
    }

    public void LoadSaveGame()
    {

    }

    public void DeleteSaveGame()
    {
        if (m_SelectedSaveGame == null)
            return;

        m_PopupWindow.SetupYesNo("Delete", "Are you sure you want to delete your save game: " + m_SelectedSaveGame.Name + "?", OnDeleteYesClicked, null);
    }

    private int FindToggleWithSaveGame(SaveGame saveGame)
    {
        for (int i = 0; i < m_SaveGameSelectToggles.Count; ++i)
        {
            if (m_SaveGameSelectToggles[i].SaveGame == saveGame)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnDeleteYesClicked()
    {
        SaveGameManager.Instance.DeleteSaveGame(m_SelectedSaveGame);
    }

    //SaveGameManager callbacks
    private void OnSaveGameAdded(SaveGame saveGame)
    {

    }

    private void OnSaveGameEdited(SaveGame saveGame)
    {

    }

    private void OnSaveGameDeleted(SaveGame saveGame)
    {
        int toggleIndex = FindToggleWithSaveGame(saveGame);

        if (toggleIndex == -1)
            return;

        SaveGameSelectToggle toggle = m_SaveGameSelectToggles[toggleIndex];

        toggle.SaveGameSelectEvent -= OnSaveGameSelect;
        GameObject.Destroy(toggle.gameObject);

        m_SaveGameSelectToggles.Remove(toggle);

        //Select a new toggle
        if (m_SelectedSaveGame == saveGame)
        {
            m_SelectedSaveGame = null;

            if (m_SaveGameSelectToggles.Count > 0)
            {
                //Try the next one
                if (toggleIndex < m_SaveGameSelectToggles.Count)
                {
                    m_SaveGameSelectToggles[toggleIndex].IsOn(true);
                    return;
                }

                //Try the previous one
                if ((toggleIndex - 1) < m_SaveGameSelectToggles.Count)
                {
                    m_SaveGameSelectToggles[toggleIndex - 1].IsOn(true);
                    return;
                }
            }
        }
    }

    private void OnSaveGamesLoaded()
    {
        //Destroy all existing toggles
        foreach (SaveGameSelectToggle toggle in m_SaveGameSelectToggles)
        {
            toggle.SaveGameSelectEvent -= OnSaveGameSelect;
            GameObject.Destroy(toggle.gameObject);
        }

        m_SaveGameSelectToggles.Clear();


        List<SaveGame> saveGamesData = SaveGameManager.Instance.SaveGames;

        for (int i = 0; i < saveGamesData.Count; ++i)
        {
            SaveGameSelectToggle toggle = GameObject.Instantiate<SaveGameSelectToggle>(m_TogglePrefab);
            toggle.Setup(saveGamesData[i], m_ContentRoot, m_ToggleGroup);
            toggle.SaveGameSelectEvent += OnSaveGameSelect;

            //Enable the first toggle
            if (i == 0)
                toggle.IsOn(true);
            else
                toggle.IsOn(false);

            m_SaveGameSelectToggles.Add(toggle);
        }
    }


}
