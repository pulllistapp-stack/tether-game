using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tether.UI
{
    /// <summary>
    /// Phase 4 pause menu. ESC toggles a pause overlay with Resume / Restart
    /// / Main Menu buttons. Uses Time.timeScale to pause everything.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                if (IsPaused) Resume(); else Pause();
            }
        }

        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0f;
            Show();
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            Hide();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        private void Show()
        {
            if (_group == null) return;
            _group.alpha = 1f;
            _group.interactable = true;
            _group.blocksRaycasts = true;
        }

        private void Hide()
        {
            if (_group == null) return;
            _group.alpha = 0f;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }
    }
}
