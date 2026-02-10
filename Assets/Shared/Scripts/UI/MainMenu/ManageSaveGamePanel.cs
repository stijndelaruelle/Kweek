using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    //TODO: Should be split into 2 classes: LoadGamePanel & SaveGamePanel.
    public class ManageSaveGamePanel : MonoBehaviour
    {
        [SerializeField]
        private SaveGameSelectToggle m_TogglePrefab = null;

        [SerializeField]
        private RectTransform m_ContentRoot = null;

        [SerializeField]
        private ToggleGroup m_ToggleGroup = null;
        private List<SaveGameSelectToggle> m_SaveGameSelectToggles = null;

        [SerializeField]
        private PopupPanel m_PopupWindow = null;

        [SerializeField]
        private ImageFader m_ImageFader = null;

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

            OnSaveGamesLoaded();
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

            if (m_SaveGameSelectToggles != null)
            {
                foreach (SaveGameSelectToggle toggle in m_SaveGameSelectToggles)
                {
                    toggle.SaveGameSelectEvent -= OnSaveGameSelect;
                }
            }
        }

        public void CreateNewSave()
        {
            SaveGame activeSaveGame = SaveGameManager.Instance.ActiveSaveGame;
            SaveGameManager.Instance.DeactivateSaveGame();

            activeSaveGame = SaveGameManager.Instance.CreateSaveGame(activeSaveGame.Name, activeSaveGame.Difficulty, LevelManager.Instance.GetCurrentLevelID(), activeSaveGame.PlayTime);
            SaveGameManager.Instance.ActivateSaveGame(activeSaveGame);
        }

        public void SaveGame()
        {
            if (m_SelectedSaveGame == null)
                return;

            m_PopupWindow.SetupYesNo("Save", "Are you sure you want to overwrite your save game: " + m_SelectedSaveGame.Name + "?", OnSaveYesClicked, null);
        }

        public void LoadSaveGame()
        {
            SaveGameManager.Instance.ActivateSaveGame(m_SelectedSaveGame);
            m_ImageFader.FadeIn(OnFadeInComplete);
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

        private void EnableToggle(SaveGameSelectToggle toggle)
        {
            foreach (SaveGameSelectToggle saveGameSelectToggle in m_SaveGameSelectToggles)
            {
                saveGameSelectToggle.IsOn((saveGameSelectToggle == toggle));
            }
        }

        //Popup callback
        private void OnSaveYesClicked()
        {
            if (m_SelectedSaveGame == null)
                return;

            SaveGameManager.Instance.EditSaveGame(m_SelectedSaveGame, SaveGameManager.Instance.ActiveSaveGame.Name, SaveGameManager.Instance.ActiveSaveGame.Difficulty, LevelManager.Instance.GetCurrentLevelID());
            SaveGameManager.Instance.ActivateSaveGame(m_SelectedSaveGame);
        }

        private void OnDeleteYesClicked()
        {
            SaveGameManager.Instance.DeleteSaveGame(m_SelectedSaveGame);
        }

        //SaveGameManager callbacks
        private void OnSaveGameAdded(SaveGame saveGame)
        {
            SaveGameSelectToggle toggle = GameObject.Instantiate<SaveGameSelectToggle>(m_TogglePrefab);
            toggle.Setup(saveGame, m_ContentRoot, m_ToggleGroup);
            toggle.SaveGameSelectEvent += OnSaveGameSelect;

            m_SaveGameSelectToggles.Add(toggle);

            //Enable the toggle
            EnableToggle(toggle);
        }

        private void OnSaveGameEdited(SaveGame saveGame)
        {
            int toggleIndex = FindToggleWithSaveGame(saveGame);

            if (toggleIndex == -1)
                return;

            SaveGameSelectToggle toggle = m_SaveGameSelectToggles[toggleIndex];

            toggle.Setup(saveGame, m_ContentRoot, m_ToggleGroup);
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
            List<SaveGame> sortedSaveGamesData = saveGamesData.OrderBy(o => o.SaveTimeStamp).ToList(); //Last saved game at the top of the list

            for (int i = 0; i < sortedSaveGamesData.Count; ++i)
            {
                SaveGameSelectToggle toggle = GameObject.Instantiate<SaveGameSelectToggle>(m_TogglePrefab);

                toggle.Setup(sortedSaveGamesData[i], m_ContentRoot, m_ToggleGroup);
                toggle.SaveGameSelectEvent += OnSaveGameSelect;

                m_SaveGameSelectToggles.Add(toggle);
            }

            //Enable the first toggle (last in the list here)
            EnableToggle(m_SaveGameSelectToggles[m_SaveGameSelectToggles.Count - 1]);
        }

        //Imagefader callback
        private void OnFadeInComplete()
        {
            //Switch to the correct scene
            LevelManager.Instance.LoadLevel(m_SelectedSaveGame.LevelID);
            m_ImageFader.SetAlphaMin();
        }

        private void OnSaveGameSelect(SaveGame saveGame)
        {
            m_SelectedSaveGame = saveGame;
        }
    }
}