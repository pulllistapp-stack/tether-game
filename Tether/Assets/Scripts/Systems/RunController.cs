using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 2 run controller. Watches PlayerHealth + WaveSystem for terminal states,
    /// exposes RunState, and handles restart on R.
    /// </summary>
    public class RunController : MonoBehaviour
    {
        public enum RunState { Playing, GameOver, Victory }

        [SerializeField] private Player.PlayerHealth _playerHealth;
        [SerializeField] private WaveSystem _waveSystem;
        [SerializeField] private KeyCode _restartKey = KeyCode.R;

        public RunState State { get; private set; } = RunState.Playing;

        public event Action<RunState> OnStateChanged;

        private void Start()
        {
            if (_playerHealth == null) _playerHealth = FindFirstObjectByType<Player.PlayerHealth>();
            if (_waveSystem == null)   _waveSystem   = FindFirstObjectByType<WaveSystem>();

            if (_playerHealth != null) _playerHealth.OnDied += HandlePlayerDied;
            if (_waveSystem   != null) _waveSystem.OnAllWavesCleared += HandleAllWavesCleared;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null) _playerHealth.OnDied -= HandlePlayerDied;
            if (_waveSystem   != null) _waveSystem.OnAllWavesCleared -= HandleAllWavesCleared;
        }

        private void Update()
        {
            if (State != RunState.Playing && Input.GetKeyDown(_restartKey))
            {
                Restart();
            }
        }

        private void HandlePlayerDied()
        {
            if (State != RunState.Playing) return;
            SetState(RunState.GameOver);
        }

        private void HandleAllWavesCleared()
        {
            if (State != RunState.Playing) return;
            SetState(RunState.Victory);
        }

        private void SetState(RunState s)
        {
            State = s;
            OnStateChanged?.Invoke(s);
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
