using UnityEngine;
using System;

namespace Tether.Enemy
{
    /// <summary>
    /// Baseline Enemy. Takes damage, dies with juice.
    /// Phase 3: Configure(EnemyData) applies visuals, HP, and mover behavior.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Fallback stats (used when EnemyData is null)")]
        [SerializeField] private float _maxHp = 5f;

        [Header("Feel (on death)")]
        [SerializeField] private float _deathShake = 0.4f;
        [SerializeField] private float _deathHitStop = 0.04f;

        [Header("Escape (missed — got past the player)")]
        [Tooltip("Enemy is destroyed and deals contact damage once it falls below this Y instead of drifting off forever. Matches Ball's void line.")]
        [SerializeField] private float _escapeY = -9f;
        [SerializeField] private int _escapeDamage = 1;

        [Header("Runtime refs")]
        [SerializeField] private EnemyData _data;
        [SerializeField] private SpriteRenderer _sprite;

        [Header("Feel (on hit)")]
        [SerializeField] private bool _showDamageNumbers = true;
        [SerializeField] private bool _showHealthBar = true;

        public event Action<Enemy> OnDeath;
        public EnemyData Data => _data;

        private float _currentHp;
        private EnemyHealthBar _healthBar;
        // Destroy() only takes effect at end of frame, so a ball collision landing the
        // same frame the enemy escapes could still call TakeDamage -> Die and hand out
        // XP/coins for an enemy that already got past the player. This guards against it.
        private bool _isGone;

        private float MaxHp => _data != null ? _data.maxHp : _maxHp;

        private void Awake()
        {
            if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
            _currentHp = MaxHp;
        }

        private void Start()
        {
            if (_showHealthBar && _sprite != null && _healthBar == null)
            {
                _healthBar = gameObject.AddComponent<EnemyHealthBar>();
                _healthBar.Build(_sprite.sprite, _sprite.sortingOrder);
            }
        }

        private void Update()
        {
            if (transform.position.y < _escapeY) Escape();
        }

        /// <summary>Punishes a miss instead of letting the enemy drift off-screen forever:
        /// hits the player once for slipping past, then disappears with no rewards
        /// (no coins/XP/combo — this isn't a kill).</summary>
        private void Escape()
        {
            if (_isGone) return;
            _isGone = true;

            var hp = FindFirstObjectByType<Player.PlayerHealth>();
            if (hp != null) hp.TakeEscapeDamage(_escapeDamage);

            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }

        public void Configure(EnemyData data)
        {
            _data = data;
            if (data == null) return;

            _currentHp = data.maxHp;
            if (_sprite != null)
            {
                if (data.spriteOverride != null)
                {
                    _sprite.sprite = data.spriteOverride;
                    _sprite.color = Color.white;
                }
                else
                {
                    _sprite.color = data.tintColor;
                }
            }
            transform.localScale = Vector3.one * data.sizeMultiplier;

            // Push movement config into EnemyMover if present
            var mover = GetComponent<EnemyMover>();
            if (mover != null) mover.ApplyData(data);

            // Warp-in visual — added after Configure so it animates toward the final scale/tint
            if (GetComponent<SpawnWarp>() == null) gameObject.AddComponent<SpawnWarp>();
        }

        public void TakeDamage(float damage)
        {
            if (_isGone) return;
            // Already past the line this same frame — a same-frame ball hit racing
            // Update()'s escape check must never resolve as a kill (no XP/coins for
            // something that got away), so route it through Escape() instead.
            if (transform.position.y < _escapeY) { Escape(); return; }
            _currentHp -= damage;

            if (_showDamageNumbers)
            {
                // Big hits read as crits so heavy-damage builds feel different
                bool crit = damage >= MaxHp * 0.5f;
                Utility.Fx.DamageNumber(transform.position, damage, crit);
            }

            if (_healthBar != null && _currentHp > 0f)
                _healthBar.SetRatio(_currentHp / Mathf.Max(0.01f, MaxHp));

            if (_currentHp <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            float max = MaxHp;
            _currentHp = Mathf.Min(max, _currentHp + Mathf.Abs(amount));
            if (_healthBar != null) _healthBar.SetRatio(_currentHp / Mathf.Max(0.01f, max));
        }

        private void Die()
        {
            if (_isGone) return;
            _isGone = true;

            OnDeath?.Invoke(this);
            Meta.SceneEnemyListener.ReportDeath();

            if (CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(_deathShake);
            if (Utility.HitStop.Instance != null)
                Utility.HitStop.Instance.Freeze(_deathHitStop);
            Audio.AudioManager.Instance?.Play("enemy_die");
            if (Systems.TimeStopSystem.Instance != null)
                Systems.TimeStopSystem.Instance.AddChargeFromEnemyKill();

            // Bloody Moon relic: heal 1 HP every 10 kills (checked via RunStats)
            if (Systems.RelicSystem.Instance != null && Systems.RelicSystem.Instance.HasRelic("bloody_moon"))
            {
                var stats = Meta.RunStats.Instance;
                if (stats != null && (stats.Kills + 1) % 10 == 0)
                {
                    var hp = FindFirstObjectByType<Player.PlayerHealth>();
                    if (hp != null) hp.Heal(1);
                }
            }

            DropCoin();
            DropXpOrb();
            SpawnSplitChildrenIfSplitter();
            SpawnDeathParticles();
            Destroy(gameObject);
        }

        private void SpawnSplitChildrenIfSplitter()
        {
            if (_data == null || !_data.isSplitter) return;
            if (_data.splitInto == null) return;

            var wave = FindFirstObjectByType<Systems.WaveSystem>();
            if (wave == null) return;

            wave.SpawnExtra(_data.splitInto, _data.splitCount, transform.position);
        }

        [Header("Loot")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private int _coinValue = 1;
        [SerializeField] private GameObject _xpOrbPrefab;

        private void DropCoin()
        {
            if (_coinPrefab == null) return;
            int baseCount = _data != null ? _data.coinReward : _coinValue;
            int bonus = Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.CoinPerKillAdd : 0;
            int coinCount = Mathf.Max(0, baseCount + bonus);
            for (int i = 0; i < coinCount; i++)
            {
                Vector2 offset = coinCount > 1
                    ? new Vector2(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f))
                    : Vector2.zero;
                Instantiate(_coinPrefab, (Vector2)transform.position + offset, Quaternion.identity);
            }
        }

        private void DropXpOrb()
        {
            if (_xpOrbPrefab == null) return;
            int baseXp = _data != null ? _data.xpReward : 1;
            int bonus = Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.XpPerKillAdd : 0;
            int xp = Mathf.Max(0, baseXp + bonus);
            if (xp <= 0) return;

            var go = Instantiate(_xpOrbPrefab, transform.position, Quaternion.identity);
            if (go.TryGetComponent<Meta.XpOrb>(out var orb)) orb.SetValue(xp);
        }

        private void SpawnDeathParticles()
        {
            if (_sprite == null || _sprite.sprite == null) return;

            const int count = 5;
            for (int i = 0; i < count; i++)
            {
                var pGO = new GameObject("DeathParticle");
                pGO.transform.position = transform.position;
                pGO.transform.localScale = Vector3.one * 0.35f;
                var psr = pGO.AddComponent<SpriteRenderer>();
                psr.sprite = _sprite.sprite;
                psr.color = _sprite.color;
                psr.sortingOrder = _sprite.sortingOrder + 1;
                var rb = pGO.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.linearDamping = 4f;
                float angle = (360f / count) * i + UnityEngine.Random.Range(-15f, 15f);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                rb.linearVelocity = dir * UnityEngine.Random.Range(3f, 5f);
                rb.angularVelocity = UnityEngine.Random.Range(-360f, 360f);
                pGO.AddComponent<Utility.FadeAndDestroy>();
            }
        }
    }
}
