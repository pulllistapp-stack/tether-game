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

        /// <summary>
        /// Global "the run is finished" gate. Static so every gameplay system can
        /// check it without holding a reference, and so it survives the frame where
        /// the controller itself is being torn down during a scene load.
        /// Nothing may award XP, offer cards, tick combos, or restore timeScale
        /// while this is true.
        /// </summary>
        public static bool IsRunOver { get; private set; }

        public event Action<RunState> OnStateChanged;

        private bool _hpSubscribed;
        private bool _waveSubscribed;

        private void Awake()
        {
            // Fresh scene load = fresh run, unless something ends it again this frame
            IsRunOver = false;
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void OnDestroy()
        {
            if (_playerHealth != null && _hpSubscribed) _playerHealth.OnDied -= HandlePlayerDied;
            if (_waveSystem   != null && _waveSubscribed) _waveSystem.OnAllWavesCleared -= HandleAllWavesCleared;
        }

        private void Update()
        {
            // Lazy subscribe in case Player / WaveSystem were spawned after our Start()
            if (!_hpSubscribed || !_waveSubscribed) TrySubscribe();

            if (State != RunState.Playing && Input.GetKeyDown(_restartKey))
            {
                Restart();
            }
        }

        private void TrySubscribe()
        {
            if (!_hpSubscribed)
            {
                if (_playerHealth == null) _playerHealth = FindFirstObjectByType<Player.PlayerHealth>();
                if (_playerHealth != null)
                {
                    _playerHealth.OnDied += HandlePlayerDied;
                    _hpSubscribed = true;
                }
            }
            if (!_waveSubscribed)
            {
                if (_waveSystem == null) _waveSystem = FindFirstObjectByType<WaveSystem>();
                if (_waveSystem != null)
                {
                    _waveSystem.OnAllWavesCleared += HandleAllWavesCleared;
                    _waveSubscribed = true;
                }
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

            var session = Meta.RunSession.Instance;
            if (session == null || session.CurrentNode == null)
            {
                // No run session (legacy standalone arena) — treat as final victory
                SetState(RunState.Victory);
                return;
            }

            session.MarkCurrentCleared();

            // Cache HP so it survives the scene reload
            var hp = FindFirstObjectByType<Player.PlayerHealth>();
            if (hp != null) session.CachePlayerHp(hp.CurrentHp, hp.MaxHp);

            if (session.CurrentNode.type == Meta.NodeType.Boss)
            {
                session.EndRun();
                SetState(RunState.Victory);
            }
            else
            {
                // Non-boss node cleared → back to map for next choice
                Time.timeScale = 1f;
                SceneManager.LoadScene("Map");
            }
        }

        private void SetState(RunState s)
        {
            State = s;
            IsRunOver = s != RunState.Playing;

            // Freeze the world when the run is over so waves/balls don't keep
            // ticking behind the overlay. Restart / GoToMainMenu reset to 1.
            if (IsRunOver)
            {
                Time.timeScale = 0f;
                // Any modal that was mid-flight (upgrade cards from a late level-up)
                // must get out of the way — the run summary owns the screen now.
                var upgradeUi = FindFirstObjectByType<UI.UpgradeUI>();
                if (upgradeUi != null) upgradeUi.Hide();

                // Clear anything still in flight so the frozen frame reads clean
                foreach (var ball in FindObjectsByType<Gameplay.Ball>(FindObjectsSortMode.None))
                    Destroy(ball.gameObject);
                foreach (var proj in FindObjectsByType<Enemy.EnemyProjectile>(FindObjectsSortMode.None))
                    Destroy(proj.gameObject);
            }
            OnStateChanged?.Invoke(s);
        }

        public void Restart()
        {
            IsRunOver = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        public void GoToMainMenu()
        {
            IsRunOver = false;
            Time.timeScale = 1f;
            // Ending a run — tear down the persisted session so a new one starts clean
            var session = Meta.RunSession.Instance;
            if (session != null) session.EndRun();
            SceneManager.LoadScene(_mainMenuSceneName);
        }
    }
}
