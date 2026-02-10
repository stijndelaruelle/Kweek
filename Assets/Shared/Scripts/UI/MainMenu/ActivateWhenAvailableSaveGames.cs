using UnityEngine;

namespace Kweek
{
    public class ActivateWhenAvailableSaveGames : MonoBehaviour
    {
        private void Start()
        {
            SaveGameManager saveGameManager = SaveGameManager.Instance;
            saveGameManager.SaveGamesLoadedEvent += OnSaveGamesLoaded;
            saveGameManager.SaveGameDeletedEvent += OnSaveGameDeleted;
        }

        private void OnDestroy()
        {
            SaveGameManager saveGameManager = SaveGameManager.Instance;
            if (saveGameManager != null)
            {
                saveGameManager.SaveGamesLoadedEvent -= OnSaveGamesLoaded;
                saveGameManager.SaveGameDeletedEvent -= OnSaveGameDeleted;
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

        private void OnSaveGameDeleted(SaveGame saveGame)
        {
            Refresh();
        }

        private void OnSaveGamesLoaded()
        {
            Refresh();
        }
    }
}