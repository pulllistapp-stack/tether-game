using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 4 per-run analytics. Observes gameplay events and stores kills,
    /// wave-reached, picked upgrades, kill combo. Read by RunSummary UI.
    /// </summary>
    public class RunStats : MonoBehaviour
    {
        public static RunStats Instance { get; private set; }

        [Header("Combo")]
        [SerializeField] private float _comboResetSeconds = 2.5f;

        public int Kills { get; private set; }
        public int WaveReached { get; private set; } = 1;
        public int CurrentCombo { get; private set; }
        public int BestCombo { get; private set; }
        public float LastKillTime { get; private set; }
        public IReadOnlyList<Systems.UpgradeCard> PickedUpgrades => _picks;

        public event Action<int, int> OnComboChanged; // (current, best)

        private readonly List<Systems.UpgradeCard> _picks = new List<Systems.UpgradeCard>();

        private Systems.WaveSystem _wave;
        private Systems.UpgradeApplier _applier;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _wave = FindFirstObjectByType<Systems.WaveSystem>();
            _applier = FindFirstObjectByType<Systems.UpgradeApplier>();

            if (_wave != null) _wave.OnWaveStarted += HandleWaveStarted;
            if (_applier != null) _applier.OnUpgradeApplied += HandleUpgradePicked;

            // Enemy events are per-instance; subscribe on spawn via a scene-wide listener
            SceneEnemyListener.OnAnyEnemyDied += HandleEnemyDied;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_wave != null) _wave.OnWaveStarted -= HandleWaveStarted;
            if (_applier != null) _applier.OnUpgradeApplied -= HandleUpgradePicked;
            SceneEnemyListener.OnAnyEnemyDied -= HandleEnemyDied;
        }

        private void Update()
        {
            if (CurrentCombo > 0 && Time.time - LastKillTime > _comboResetSeconds)
            {
                CurrentCombo = 0;
                OnComboChanged?.Invoke(CurrentCombo, BestCombo);
            }
        }

        private void HandleWaveStarted(int idx, int total)
        {
            WaveReached = idx + 1;
        }

        private void HandleUpgradePicked(Systems.UpgradeCard card)
        {
            _picks.Add(card);
        }

        private void HandleEnemyDied()
        {
            Kills++;
            CurrentCombo++;
            LastKillTime = Time.time;
            if (CurrentCombo > BestCombo) BestCombo = CurrentCombo;
            OnComboChanged?.Invoke(CurrentCombo, BestCombo);
        }
    }

    /// <summary>
    /// Simple static aggregator: any Enemy fires this on death so RunStats
    /// (and future observers) don't need to subscribe per-instance.
    /// </summary>
    public static class SceneEnemyListener
    {
        public static event Action OnAnyEnemyDied;
        public static void ReportDeath() => OnAnyEnemyDied?.Invoke();
    }
}
