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
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private MetaShopController _shop;

        private const string PrefTotal    = "tether.total_coins";
        private const string PrefBestWave = "tether.best_wave";
        private const string PrefBestCombo = "tether.best_combo";

        private void Start()
        {
            Time.timeScale = 1f;

            // Auto-find buttons/shop by name if not wired
            if (_startButton == null || _shopButton == null || _quitButton == null || _shop == null)
            {
                foreach (var b in FindObjectsByType<Button>(FindObjectsSortMode.None))
                {
                    if (_startButton == null && b.name.IndexOf("START", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        _startButton = b;
                    else if (_shopButton == null && b.name.IndexOf("SHOP", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        _shopButton = b;
                    else if (_quitButton == null && b.name.IndexOf("QUIT", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        _quitButton = b;
                }
                if (_shop == null) _shop = FindFirstObjectByType<MetaShopController>();
            }

            if (_startButton != null) _startButton.onClick.AddListener(StartRun);
            if (_quitButton != null)  _quitButton.onClick.AddListener(Quit);
            if (_shopButton != null && _shop != null) _shopButton.onClick.AddListener(_shop.Show);

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
