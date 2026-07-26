using UnityEngine;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 5 PlayerPrefs facade for lifetime coins + unlockable flags.
    /// Central so tests / debug can wipe / seed values in one place.
    /// </summary>
    public static class MetaSave
    {
        public const string PrefTotalCoins = "tether.total_coins";
        public const string PrefBestWave = "tether.best_wave";
        public const string PrefBestCombo = "tether.best_combo";
        private const string UnlockPrefix = "tether.unlock.";

        public static int LifetimeCoins
        {
            get => PlayerPrefs.GetInt(PrefTotalCoins, 0);
            set { PlayerPrefs.SetInt(PrefTotalCoins, Mathf.Max(0, value)); PlayerPrefs.Save(); }
        }

        public static bool IsUnlocked(string id) =>
            PlayerPrefs.GetInt(UnlockPrefix + id, 0) == 1;

        public static void SetUnlocked(string id, bool value)
        {
            PlayerPrefs.SetInt(UnlockPrefix + id, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>Try to spend coins from lifetime total. Returns true on success.</summary>
        public static bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            int have = LifetimeCoins;
            if (have < amount) return false;
            LifetimeCoins = have - amount;
            return true;
        }

        public static void Grant(int coins) => LifetimeCoins = LifetimeCoins + coins;
    }
}
