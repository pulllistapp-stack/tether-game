using UnityEngine;
using UnityEngine.UI;

namespace Tether.UI
{
    /// <summary>
    /// Phase 2 HUD. Polls PlayerHealth + WaveSystem + RunController and updates
    /// simple text labels. Shows overlay text on game-over / victory.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player.PlayerHealth _playerHealth;
        [SerializeField] private Player.BallSlotManager _ballSlots;
        [SerializeField] private Player.PlayerDash _dash;
        [SerializeField] private Player.NovaBomb _nova;
        [SerializeField] private Systems.WaveSystem _waveSystem;
        [SerializeField] private Systems.RunController _runController;

        [Header("UI")]
        [SerializeField] private Text _hpLabel;
        [SerializeField] private Text _waveLabel;
        [SerializeField] private Text _enemiesLabel;
        [SerializeField] private Text _ballLabel;
        [SerializeField] private Text _coinsLabel;
        [SerializeField] private Text _comboLabel;
        [SerializeField] private Text _dashLabel;
        [SerializeField] private Text _novaLabel;
        [SerializeField] private CanvasGroup _overlayGroup;
        [SerializeField] private Text _overlayTitle;
        [SerializeField] private Text _overlayHint;

        private void Start()
        {
            if (_playerHealth == null) _playerHealth = FindFirstObjectByType<Player.PlayerHealth>();
            if (_ballSlots == null)    _ballSlots    = FindFirstObjectByType<Player.BallSlotManager>();
            if (_dash == null)         _dash         = FindFirstObjectByType<Player.PlayerDash>();
            if (_nova == null)         _nova         = FindFirstObjectByType<Player.NovaBomb>();
            if (_waveSystem == null)   _waveSystem   = FindFirstObjectByType<Systems.WaveSystem>();
            if (_runController == null) _runController = FindFirstObjectByType<Systems.RunController>();

            if (_runController != null) _runController.OnStateChanged += HandleStateChanged;
            HideOverlay();
        }

        private void OnDestroy()
        {
            if (_runController != null) _runController.OnStateChanged -= HandleStateChanged;
        }

        private void Update()
        {
            if (_hpLabel != null && _playerHealth != null)
                _hpLabel.text = "HP  " + _playerHealth.CurrentHp + " / " + _playerHealth.MaxHp;

            if (_waveLabel != null && _waveSystem != null)
            {
                int idx = Mathf.Max(0, _waveSystem.CurrentWaveIndex) + 1;
                _waveLabel.text = "WAVE  " + idx + " / " + _waveSystem.TotalWaves;
            }

            if (_enemiesLabel != null && _waveSystem != null)
                _enemiesLabel.text = "ENEMIES  " + _waveSystem.EnemiesAlive;

            if (_ballLabel != null && _ballSlots != null && _ballSlots.CurrentData != null)
                _ballLabel.text = "[" + (_ballSlots.CurrentIndex + 1) + "] " + _ballSlots.CurrentData.displayName.ToUpper();

            if (_coinsLabel != null && Meta.CoinWallet.Instance != null)
                _coinsLabel.text = "COINS  " + Meta.CoinWallet.Instance.RunCoins;

            if (_comboLabel != null && Meta.RunStats.Instance != null)
            {
                int c = Meta.RunStats.Instance.CurrentCombo;
                _comboLabel.text = c >= 2 ? c + "x COMBO" : "";
                if (c >= 2)
                {
                    float hue = Mathf.Clamp01(c / 30f);
                    _comboLabel.color = Color.Lerp(new Color(1f, 0.9f, 0.4f), new Color(1f, 0.4f, 0.2f), hue);
                }
            }

            if (_dashLabel != null && _dash != null)
                _dashLabel.text = _dash.IsReady ? "DASH  <space>" : "DASH  " + Mathf.Max(0f, _dash.Cooldown - _dash.TimeSinceLastDash).ToString("F1") + "s";

            if (_novaLabel != null && _nova != null)
                _novaLabel.text = _nova.IsReady ? "NOVA  <e>" : "NOVA  " + Mathf.Max(0f, _nova.Cooldown - _nova.TimeSinceLastFire).ToString("F1") + "s";
        }

        private void HandleStateChanged(Systems.RunController.RunState s)
        {
            switch (s)
            {
                case Systems.RunController.RunState.GameOver:
                    ShowOverlay("GAME OVER", BuildSummary() + "\n\nPress R to restart  •  Press ESC for menu");
                    CommitRunResult();
                    break;
                case Systems.RunController.RunState.Victory:
                    ShowOverlay("VICTORY", BuildSummary() + "\n\nPress R to run it back  •  Press ESC for menu");
                    CommitRunResult();
                    break;
                default:
                    HideOverlay();
                    break;
            }
        }

        private string BuildSummary()
        {
            var stats = Meta.RunStats.Instance;
            var wallet = Meta.CoinWallet.Instance;
            if (stats == null) return "";

            var sb = new System.Text.StringBuilder();
            sb.Append("WAVE  ").Append(stats.WaveReached).Append(" / ")
              .Append(_waveSystem != null ? _waveSystem.TotalWaves : 0).AppendLine();
            sb.Append("KILLS  ").Append(stats.Kills).AppendLine();
            sb.Append("BEST COMBO  ").Append(stats.BestCombo).Append("x").AppendLine();
            if (wallet != null)
                sb.Append("COINS  ").Append(wallet.RunCoins)
                  .Append("  (lifetime  ").Append(wallet.LifetimeCoins + wallet.RunCoins).Append(")").AppendLine();

            if (stats.PickedUpgrades != null && stats.PickedUpgrades.Count > 0)
            {
                sb.AppendLine().Append("BUILD:  ");
                bool first = true;
                foreach (var c in stats.PickedUpgrades)
                {
                    if (!first) sb.Append("  •  ");
                    sb.Append(c.displayName);
                    first = false;
                }
            }
            return sb.ToString();
        }

        private bool _resultsCommitted;
        private void CommitRunResult()
        {
            if (_resultsCommitted) return;
            _resultsCommitted = true;

            var wallet = Meta.CoinWallet.Instance;
            var stats = Meta.RunStats.Instance;
            if (wallet != null && stats != null)
            {
                wallet.ReportRunResult(stats.WaveReached, stats.BestCombo);
                wallet.CommitRunToLifetime();
            }
        }

        private void ShowOverlay(string title, string hint)
        {
            if (_overlayGroup == null) return;
            _overlayGroup.alpha = 1f;
            _overlayGroup.blocksRaycasts = false;
            if (_overlayTitle != null) _overlayTitle.text = title;
            if (_overlayHint  != null) _overlayHint.text  = hint;
        }

        private void HideOverlay()
        {
            if (_overlayGroup == null) return;
            _overlayGroup.alpha = 0f;
            _overlayGroup.blocksRaycasts = false;
        }
    }
}
