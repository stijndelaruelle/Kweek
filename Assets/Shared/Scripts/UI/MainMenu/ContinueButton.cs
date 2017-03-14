using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    [SerializeField]
    private ImageFader m_ImageFader;

    private void Start()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;

        saveGameManager.SaveGameAddedEvent += OnSaveGameAdded;
        saveGameManager.SaveGameDeletedEvent += OnSaveGameDeleted;
        saveGameManager.SaveGamesLoadedEvent += OnSaveGamesLoaded;

        //In case loading takes a long times
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SaveGameManager saveGameManager = SaveGameManager.Instance;
        if (saveGameManager != null)
        {
            saveGameManager.SaveGameAddedEvent -= OnSaveGameAdded;
            saveGameManager.SaveGameDeletedEvent -= OnSaveGameDeleted;
            saveGameManager.SaveGamesLoadedEvent -= OnSaveGamesLoaded;
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Continue()
    {
        SaveGameManager.Instance.ActivateMostRecent();
        m_ImageFader.FadeIn(OnFadeInComplete);
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
    private void OnSaveGameAdded(SaveGame saveGame)
    {
        //Refresh();
    }

    private void OnSaveGameDeleted(SaveGame saveGame)
    {
        Refresh();
    }

    private void OnSaveGamesLoaded()
    {
        Refresh();
    }

    private void OnFadeInComplete()
    {
        LevelManager.Instance.LoadLevel(SaveGameManager.Instance.ActiveSaveGame.LevelID);
        m_ImageFader.SetAlphaMin();
    }
}
