using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 2 wave system. Spawns a sequence of waves with scaling difficulty.
    /// Waits for all enemies destroyed before advancing. Emits win event when done.
    /// </summary>
    public class WaveSystem : MonoBehaviour
    {
        [Serializable]
        public struct WaveDefinition
        {
            public int enemyCount;
            [Tooltip("Multiplier applied to enemy movement speed.")]
            public float enemySpeedMultiplier;
            [Tooltip("Delay before the wave begins spawning.")]
            public float startDelay;
            [Tooltip("Interval between individual spawns.")]
            public float spawnInterval;
        }

        [Header("Spawn")]
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Vector2 _spawnAreaMin = new Vector2(-4f, 5f);
        [SerializeField] private Vector2 _spawnAreaMax = new Vector2(4f, 7f);

        [Header("Waves")]
        [SerializeField] private WaveDefinition[] _waves = new WaveDefinition[] {
            new WaveDefinition { enemyCount = 4, enemySpeedMultiplier = 1.0f, startDelay = 1.5f, spawnInterval = 0.35f },
            new WaveDefinition { enemyCount = 6, enemySpeedMultiplier = 1.2f, startDelay = 2.5f, spawnInterval = 0.30f },
            new WaveDefinition { enemyCount = 9, enemySpeedMultiplier = 1.4f, startDelay = 3.0f, spawnInterval = 0.25f },
        };

        public int CurrentWaveIndex { get; private set; } = -1;
        public int TotalWaves => _waves.Length;
        public int EnemiesAlive => _tracked.Count;
        public bool AllWavesCleared { get; private set; }

        public event Action<int, int> OnWaveStarted;
        public event Action<int> OnWaveCleared;
        public event Action OnAllWavesCleared;

        private readonly List<Enemy.Enemy> _tracked = new List<Enemy.Enemy>();
        private Coroutine _routine;

        private void Start()
        {
            _routine = StartCoroutine(RunAllWaves());
        }

        private IEnumerator RunAllWaves()
        {
            for (int i = 0; i < _waves.Length; i++)
            {
                CurrentWaveIndex = i;
                yield return StartCoroutine(RunWave(i));
                OnWaveCleared?.Invoke(i);
            }
            AllWavesCleared = true;
            OnAllWavesCleared?.Invoke();
            _routine = null;
        }

        private IEnumerator RunWave(int index)
        {
            var w = _waves[index];
            yield return new WaitForSeconds(w.startDelay);
            OnWaveStarted?.Invoke(index, _waves.Length);

            for (int i = 0; i < w.enemyCount; i++)
            {
                SpawnOne(w.enemySpeedMultiplier);
                if (w.spawnInterval > 0f)
                    yield return new WaitForSeconds(w.spawnInterval);
            }

            // Wait until all tracked enemies are gone
            while (_tracked.Count > 0)
            {
                _tracked.RemoveAll(e => e == null);
                yield return null;
            }
        }

        private void SpawnOne(float speedMul)
        {
            if (_enemyPrefab == null) return;

            Vector2 pos = new Vector2(
                UnityEngine.Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                UnityEngine.Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
            );
            var go = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            if (go.TryGetComponent<Enemy.Enemy>(out var enemy))
            {
                _tracked.Add(enemy);
                enemy.OnDeath += HandleEnemyDeath;
            }
            if (go.TryGetComponent<Enemy.EnemyMover>(out var mover))
            {
                mover.SetSpeedMultiplier(speedMul);
            }
        }

        private void HandleEnemyDeath(Enemy.Enemy e)
        {
            _tracked.Remove(e);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0.15f, 0.6f);
            Vector3 center = new Vector3(
                (_spawnAreaMin.x + _spawnAreaMax.x) * 0.5f,
                (_spawnAreaMin.y + _spawnAreaMax.y) * 0.5f, 0f);
            Vector3 size = new Vector3(
                _spawnAreaMax.x - _spawnAreaMin.x,
                _spawnAreaMax.y - _spawnAreaMin.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
