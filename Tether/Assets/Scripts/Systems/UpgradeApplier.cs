using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 3 upgrade state accumulator. Singleton that holds active
    /// modifier stacks. Ball/Player systems query this at appropriate
    /// times (spawn / fire / max-hp init) to compose final values.
    /// </summary>
    public class UpgradeApplier : MonoBehaviour
    {
        public static UpgradeApplier Instance { get; private set; }

        // Multipliers start at 1.0
        public float BallDamageMul { get; private set; } = 1f;
        public float BallSpeedMul { get; private set; } = 1f;
        public float BallSizeMul { get; private set; } = 1f;
        public float FireCooldownMul { get; private set; } = 1f;
        public float ExplosionRadiusMul { get; private set; } = 1f;

        // Additives start at 0
        public int ExtraProjectiles { get; private set; } = 0;
        public int PlayerMaxHpAdd { get; private set; } = 0;
        public int SplitCountAdd { get; private set; } = 0;
        public int SplitDepthAdd { get; private set; } = 0;
        public int BounceExtensionAdd { get; private set; } = 0;
        public int CoinPerKillAdd { get; private set; } = 0;
        public int XpPerKillAdd { get; private set; } = 0;

        public event Action<UpgradeCard> OnUpgradeApplied;

        private readonly List<UpgradeCard> _picked = new List<UpgradeCard>();
        public IReadOnlyList<UpgradeCard> Picked => _picked;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Apply(UpgradeCard card)
        {
            if (card == null) return;
            _picked.Add(card);

            // Relic-grant cards bypass the effect switch and just hand the relic
            // to the RelicSystem; effect field is ignored.
            if (card.grantRelic != null && RelicSystem.Instance != null)
            {
                RelicSystem.Instance.Grant(card.grantRelic);
                OnUpgradeApplied?.Invoke(card);
                return;
            }

            switch (card.effect)
            {
                case UpgradeEffect.BallDamage:       BallDamageMul *= card.magnitude; break;
                case UpgradeEffect.BallSpeed:        BallSpeedMul *= card.magnitude; break;
                case UpgradeEffect.BallSize:         BallSizeMul *= card.magnitude; break;
                case UpgradeEffect.FireCooldown:     FireCooldownMul *= card.magnitude; break;
                case UpgradeEffect.ExplosionRadius:  ExplosionRadiusMul *= card.magnitude; break;

                case UpgradeEffect.ExtraProjectile:  ExtraProjectiles += Mathf.RoundToInt(card.magnitude); break;
                case UpgradeEffect.PlayerMaxHp:      PlayerMaxHpAdd += Mathf.RoundToInt(card.magnitude); break;
                case UpgradeEffect.SplitCount:       SplitCountAdd += Mathf.RoundToInt(card.magnitude); break;
                case UpgradeEffect.SplitDepth:       SplitDepthAdd += Mathf.RoundToInt(card.magnitude); break;
                case UpgradeEffect.BounceExtension:  BounceExtensionAdd += Mathf.RoundToInt(card.magnitude); break;
                case UpgradeEffect.CoinPerKill:      CoinPerKillAdd += Mathf.RoundToInt(card.magnitude); break;
                case UpgradeEffect.XpPerKill:        XpPerKillAdd += Mathf.RoundToInt(card.magnitude); break;

                case UpgradeEffect.PlayerHeal:
                {
                    var hp = FindFirstObjectByType<Player.PlayerHealth>();
                    if (hp != null) hp.Heal(Mathf.RoundToInt(card.magnitude));
                    break;
                }
            }

            // Live max-hp bump when a MaxHp card is picked
            if (card.effect == UpgradeEffect.PlayerMaxHp)
            {
                var hp = FindFirstObjectByType<Player.PlayerHealth>();
                if (hp != null) hp.ExtendMaxHp(Mathf.RoundToInt(card.magnitude));
            }

            OnUpgradeApplied?.Invoke(card);
        }
    }
}
