using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tether.UI
{
    /// <summary>
    /// Phase 4 main menu controller. Reads lifetime stats from PlayerPrefs
    /// (via CoinWallet-owned keys) and shows them alongside Start Run.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string _arenaSceneName = "Arena_Test";
        [SerializeField] private Text _statsLabel;
        [SerializeField] private Text _versionLabel;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;

        private const string PrefTotal    = "tether.total_coins";
        private const string PrefBestWave = "tether.best_wave";
        private const string PrefBestCombo = "tether.best_combo";

        private void Start()
        {
            Time.timeScale = 1f;

            if (_startButton != null) _startButton.onClick.AddListener(StartRun);
            if (_quitButton != null)  _quitButton.onClick.AddListener(Quit);

            if (_statsLabel != null)
            {
                int coins = PlayerPrefs.GetInt(PrefTotal, 0);
                int wave = PlayerPrefs.GetInt(PrefBestWave, 0);
                int combo = PlayerPrefs.GetInt(PrefBestCombo, 0);
                _statsLabel.text = string.Format(
                    "LIFETIME COINS  {0}\nBEST WAVE  {1}\nBEST COMBO  {2}x",
                    coins, wave, combo);
            }

            if (_versionLabel != null) _versionLabel.text = "TETHER  v0.4";
        }

        public void StartRun()
        {
            SceneManager.LoadScene(_arenaSceneName);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
