using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 5 in-run leveling. Enemies drop XP orbs; picking them up feeds
    /// AddXp(). When threshold is reached, fires OnLeveledUp which the
    /// UpgradeSelector uses to offer a mid-run upgrade card set.
    ///
    /// XP curve: level N requires (N * _xpPerLevelBase) XP. Overflow XP
    /// carries into the next level.
    /// </summary>
    public class LevelSystem : MonoBehaviour
    {
        public static LevelSystem Instance { get; private set; }

        [SerializeField] private int _xpPerLevelBase = 5;
        [Tooltip("Each level's requirement grows by this multiplier: level N needs base * N * mul.")]
        [SerializeField] private float _levelSlope = 1.0f;

        public int Level { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int XpToNext => XpRequiredFor(Level);
        public float LevelProgress01 => Mathf.Clamp01((float)CurrentXp / Mathf.Max(1, XpToNext));

        public event Action<int> OnXpChanged; // (currentXp)
        public event Action<int> OnLeveledUp; // (newLevel)

        // Pending level-ups queue (in case multiple orbs land in one frame)
        private readonly Queue<int> _pendingLevelUps = new Queue<int>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void AddXp(int amount)
        {
            if (amount <= 0) return;
            CurrentXp += amount;

            while (CurrentXp >= XpRequiredFor(Level))
            {
                CurrentXp -= XpRequiredFor(Level);
                Level++;
                _pendingLevelUps.Enqueue(Level);
            }

            OnXpChanged?.Invoke(CurrentXp);

            // Fire level-up events after XP change so listeners see final state
            while (_pendingLevelUps.Count > 0)
            {
                int lvl = _pendingLevelUps.Dequeue();
                OnLeveledUp?.Invoke(lvl);
            }
        }

        public int XpRequiredFor(int level) =>
            Mathf.Max(1, Mathf.RoundToInt(_xpPerLevelBase * level * _levelSlope));
    }
}
