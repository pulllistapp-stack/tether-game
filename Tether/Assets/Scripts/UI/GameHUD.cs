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
        [SerializeField] private Systems.WaveSystem _waveSystem;
        [SerializeField] private Systems.RunController _runController;

        [Header("UI")]
        [SerializeField] private Text _hpLabel;
        [SerializeField] private Text _waveLabel;
        [SerializeField] private Text _enemiesLabel;
        [SerializeField] private Text _ballLabel;
        [SerializeField] private CanvasGroup _overlayGroup;
        [SerializeField] private Text _overlayTitle;
        [SerializeField] private Text _overlayHint;

        private void Start()
        {
            if (_playerHealth == null) _playerHealth = FindFirstObjectByType<Player.PlayerHealth>();
            if (_ballSlots == null)    _ballSlots    = FindFirstObjectByType<Player.BallSlotManager>();
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
        }

        private void HandleStateChanged(Systems.RunController.RunState s)
        {
            switch (s)
            {
                case Systems.RunController.RunState.GameOver:
                    ShowOverlay("GAME OVER", "Press R to restart");
                    break;
                case Systems.RunController.RunState.Victory:
                    ShowOverlay("VICTORY", "Press R to run it back");
                    break;
                default:
                    HideOverlay();
                    break;
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
