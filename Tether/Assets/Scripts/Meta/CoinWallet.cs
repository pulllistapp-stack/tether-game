using UnityEngine;
using System;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 4 currency singleton. Tracks coins earned this run and syncs
    /// a lifetime total via PlayerPrefs.
    /// </summary>
    public class CoinWallet : MonoBehaviour
    {
        public static CoinWallet Instance { get; private set; }

        private const string PrefTotal = "tether.total_coins";
        private const string PrefBestWave = "tether.best_wave";
        private const string PrefBestCombo = "tether.best_combo";

        public int RunCoins { get; private set; }
        public int LifetimeCoins => PlayerPrefs.GetInt(PrefTotal, 0);
        public int BestWave => PlayerPrefs.GetInt(PrefBestWave, 0);
        public int BestCombo => PlayerPrefs.GetInt(PrefBestCombo, 0);

        public event Action<int> OnRunCoinsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void AddCoin(int amount = 1)
        {
            RunCoins += amount;
            OnRunCoinsChanged?.Invoke(RunCoins);
        }

        public void CommitRunToLifetime()
        {
            PlayerPrefs.SetInt(PrefTotal, LifetimeCoins + RunCoins);
            PlayerPrefs.Save();
        }

        public void ReportRunResult(int waveReached, int bestCombo)
        {
            if (waveReached > BestWave)  PlayerPrefs.SetInt(PrefBestWave, waveReached);
            if (bestCombo > BestCombo)   PlayerPrefs.SetInt(PrefBestCombo, bestCombo);
            PlayerPrefs.Save();
        }
    }
}
