using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tether.UI
{
    /// <summary>
    /// Pause menu. ESC toggles a pause overlay with Resume / Restart / Main
    /// Menu buttons. Buttons are wired at Awake by finding named children —
    /// this keeps them working across scene reloads without relying on
    /// persistent onClick UnityEvents.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        [Header("Buttons (auto-found by name if unset)")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            Hide();
            AutoFindButtons();
            WireButtons();
        }

        private void AutoFindButtons()
        {
            if (_resumeButton == null)   _resumeButton   = FindChildButton("RESUME");
            if (_restartButton == null)  _restartButton  = FindChildButton("RESTART");
            if (_mainMenuButton == null) _mainMenuButton = FindChildButton("MAIN MENU");
        }

        private Button FindChildButton(string labelSubstring)
        {
            foreach (var b in GetComponentsInChildren<Button>(true))
            {
                if (b.name.IndexOf(labelSubstring, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return b;
            }
            return null;
        }

        private void WireButtons()
        {
            if (_resumeButton != null)
            {
                _resumeButton.onClick.RemoveListener(Resume);
                _resumeButton.onClick.AddListener(Resume);
            }
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(Restart);
                _restartButton.onClick.AddListener(Restart);
            }
            if (_mainMenuButton != null)
            {
                _mainMenuButton.onClick.RemoveListener(GoToMainMenu);
                _mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
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
