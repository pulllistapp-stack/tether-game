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

        [Header("Runtime refs")]
        [SerializeField] private EnemyData _data;
        [SerializeField] private SpriteRenderer _sprite;

        public event Action<Enemy> OnDeath;
        public EnemyData Data => _data;

        private float _currentHp;

        private void Awake()
        {
            if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
            _currentHp = _data != null ? _data.maxHp : _maxHp;
        }

        public void Configure(EnemyData data)
        {
            _data = data;
            if (data == null) return;

            _currentHp = data.maxHp;
            if (_sprite != null) _sprite.color = data.tintColor;
            transform.localScale = Vector3.one * data.sizeMultiplier;

            // Push movement config into EnemyMover if present
            var mover = GetComponent<EnemyMover>();
            if (mover != null) mover.ApplyData(data);
        }

        public void TakeDamage(float damage)
        {
            _currentHp -= damage;
            if (_currentHp <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDeath?.Invoke(this);
            Meta.SceneEnemyListener.ReportDeath();

            if (CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(_deathShake);
            if (Utility.HitStop.Instance != null)
                Utility.HitStop.Instance.Freeze(_deathHitStop);
            Audio.AudioManager.Instance?.Play("enemy_die");

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
            int coinCount = _data != null ? _data.coinReward : _coinValue;
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
            int xp = _data != null ? _data.xpReward : 1;
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
