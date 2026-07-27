using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 3 upgrade selector. Between waves: pause game (Time.timeScale = 0),
    /// draw 3 weighted cards from the pool, and show them via UpgradeUI.
    /// UpgradeUI reports the pick back through a callback; we apply and resume.
    /// </summary>
    public class UpgradeSelector : MonoBehaviour
    {
        [SerializeField] private UpgradeCard[] _cardPool;
        [SerializeField] private WaveSystem _waveSystem;
        [SerializeField] private UpgradeApplier _applier;
        [SerializeField] private UI.UpgradeUI _ui;
        [SerializeField] private int _cardsPerOffer = 3;

        public event Action OnOffered;
        public event Action OnPicked;

        private Meta.LevelSystem _levelSystem;
        private bool _modalOpen;
        private readonly System.Collections.Generic.Queue<string> _pendingOffers =
            new System.Collections.Generic.Queue<string>();

        private void Start()
        {
            if (_waveSystem == null) _waveSystem = FindFirstObjectByType<WaveSystem>();
            if (_applier == null)    _applier    = FindFirstObjectByType<UpgradeApplier>();
            if (_ui == null)         _ui         = FindFirstObjectByType<UI.UpgradeUI>();
            if (_levelSystem == null) _levelSystem = FindFirstObjectByType<Meta.LevelSystem>();

            if (_waveSystem != null) _waveSystem.OnWaveCleared += HandleWaveCleared;
            if (_levelSystem != null) _levelSystem.OnLeveledUp += HandleLeveledUp;
        }

        private void OnDestroy()
        {
            if (_waveSystem != null) _waveSystem.OnWaveCleared -= HandleWaveCleared;
            if (_levelSystem != null) _levelSystem.OnLeveledUp -= HandleLeveledUp;
        }

        private void HandleWaveCleared(int index)
        {
            // Skip after final wave — VictoryUI handles that
            if (_waveSystem == null || index >= _waveSystem.TotalWaves - 1) return;
            TryOffer("wave");
        }

        private void HandleLeveledUp(int newLevel)
        {
            TryOffer("level");
        }

        private void TryOffer(string source)
        {
            // A late level-up (XP orb landing as the boss dies) must not throw a
            // card modal over the run summary.
            if (RunController.IsRunOver) return;

            if (_modalOpen)
            {
                _pendingOffers.Enqueue(source);
                return;
            }
            OfferCards();
        }

        private void OfferCards()
        {
            if (RunController.IsRunOver) return;
            if (_ui == null || _cardPool == null || _cardPool.Length == 0) return;

            var picks = WeightedDraw(_cardPool, _cardsPerOffer);
            if (picks.Count == 0) return;

            _modalOpen = true;
            _ui.Show(picks, HandleCardPicked);
            OnOffered?.Invoke();
            // Defer pause past any active HitStop so its restore-to-1 doesn't wipe us
            StartCoroutine(EngagePauseAfterHitStop());
        }

        private IEnumerator EngagePauseAfterHitStop()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (RunController.IsRunOver) yield break;
            Time.timeScale = 0f;
        }

        private void HandleCardPicked(UpgradeCard card)
        {
            if (_applier != null) _applier.Apply(card);
            _ui.Hide();
            _modalOpen = false;
            OnPicked?.Invoke();

            // Never hand time back to a finished run — the summary screen owns the freeze
            if (RunController.IsRunOver)
            {
                _pendingOffers.Clear();
                return;
            }
            Time.timeScale = 1f;

            // Drain any queued offers (e.g., double level-up during a wave clear)
            if (_pendingOffers.Count > 0)
            {
                _pendingOffers.Dequeue();
                OfferCards();
            }
        }

        private static List<UpgradeCard> WeightedDraw(UpgradeCard[] pool, int count)
        {
            var remaining = new List<UpgradeCard>(pool);
            var drawn = new List<UpgradeCard>(count);

            for (int i = 0; i < count && remaining.Count > 0; i++)
            {
                int totalWeight = 0;
                foreach (var c in remaining) totalWeight += Mathf.Max(1, c.weight);
                int roll = UnityEngine.Random.Range(0, totalWeight);
                int running = 0;
                for (int k = 0; k < remaining.Count; k++)
                {
                    running += Mathf.Max(1, remaining[k].weight);
                    if (roll < running)
                    {
                        drawn.Add(remaining[k]);
                        remaining.RemoveAt(k);
                        break;
                    }
                }
            }
            return drawn;
        }
    }
}
