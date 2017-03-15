using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ActivateWhenAvailableSaveGames : MonoBehaviour
{

    private void Start()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;
        saveGameManager.SaveGamesLoadedEvent += OnSaveGamesLoaded;
    }

    private void OnDestroy()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;
        if (saveGameManager != null)
        {
            saveGameManager.SaveGamesLoadedEvent -= OnSaveGamesLoaded;
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;
        if (saveGameManager != null)
        {
            bool enabled = (saveGameManager.GetSaveGameCount() > 0);
            gameObject.SetActive(enabled);
        }
    }

    //SaveGameManager callbacks
    //private void OnSaveGameAdded(SaveGame saveGame)
    //{
    //    //Refresh();
    //}

    //private void OnSaveGameDeleted(SaveGame saveGame)
    //{
    //    Refresh();
    //}

    private void OnSaveGamesLoaded()
    {
        Refresh();
    }
}
