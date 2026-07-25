using UnityEngine;
using UnityEngine.InputSystem;

namespace Tether.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        public enum Tab { Map, Collection, Controls }

        [Header("Panels")]
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _mapPanel;
        [SerializeField] private GameObject _collectionPanel;
        [SerializeField] private GameObject _controlsPanel;

        [Header("Settings")]
        [SerializeField] private bool _pauseGameTime = true;

        public static PauseMenuController Instance { get; private set; }
        public bool IsPaused { get; private set; }
        public Tab CurrentTab { get; private set; } = Tab.Map;

        public System.Action<bool> OnPauseChanged;
        public System.Action<Tab> OnTabChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (_pausePanel != null) _pausePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (IsPaused) Resume();
            else Pause();
        }

        public void Pause()
        {
            if (IsPaused) return;
            IsPaused = true;
            if (_pauseGameTime) Time.timeScale = 0f;
            if (_pausePanel != null) _pausePanel.SetActive(true);
            ShowTab(Tab.Map);
            OnPauseChanged?.Invoke(true);
        }

        public void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            if (_pauseGameTime) Time.timeScale = 1f;
            if (_pausePanel != null) _pausePanel.SetActive(false);
            OnPauseChanged?.Invoke(false);
        }

        public void ShowMap() => ShowTab(Tab.Map);
        public void ShowCollection() => ShowTab(Tab.Collection);
        public void ShowControls() => ShowTab(Tab.Controls);

        public void ShowTab(Tab tab)
        {
            CurrentTab = tab;
            if (_mapPanel != null) _mapPanel.SetActive(tab == Tab.Map);
            if (_collectionPanel != null) _collectionPanel.SetActive(tab == Tab.Collection);
            if (_controlsPanel != null) _controlsPanel.SetActive(tab == Tab.Controls);
            OnTabChanged?.Invoke(tab);
        }

        public void QuitToMenu()
        {
            Resume();
        }
    }
}
