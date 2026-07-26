using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 3 wave system. Each wave defines a mix of EnemyData + count.
    /// Spawns are shuffled within a wave for visual variety. Waits for all
    /// enemies destroyed before firing OnWaveCleared. Upgrade system can
    /// pause with Time.timeScale to gate advancement between waves.
    /// </summary>
    public class WaveSystem : MonoBehaviour
    {
        [Serializable]
        public struct WaveEnemyEntry
        {
            public Enemy.EnemyData data;
            public int count;
        }

        [Serializable]
        public struct WaveDefinition
        {
            public string label;
            public WaveEnemyEntry[] mix;
            [Tooltip("Multiplier applied to enemy fall speed.")]
            public float enemySpeedMultiplier;
            [Tooltip("Delay before the wave begins spawning (real world seconds passed to WaitForSeconds; pauses if Time.timeScale = 0).")]
            public float startDelay;
            [Tooltip("Interval between individual spawns.")]
            public float spawnInterval;
        }

        [Header("Spawn")]
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Vector2 _spawnAreaMin = new Vector2(-4f, 5f);
        [SerializeField] private Vector2 _spawnAreaMax = new Vector2(4f, 7f);

        [Header("Waves")]
        [SerializeField] private WaveDefinition[] _waves = new WaveDefinition[0];

        public int CurrentWaveIndex { get; private set; } = -1;
        public int TotalWaves => _waves != null ? _waves.Length : 0;
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

            // Build spawn list from mix, shuffle for variety
            var spawnList = new List<Enemy.EnemyData>();
            if (w.mix != null)
            {
                foreach (var entry in w.mix)
                    for (int k = 0; k < entry.count; k++)
                        spawnList.Add(entry.data);
            }
            Shuffle(spawnList);

            foreach (var data in spawnList)
            {
                SpawnOne(data, w.enemySpeedMultiplier);
                if (w.spawnInterval > 0f)
                    yield return new WaitForSeconds(w.spawnInterval);
            }

            while (_tracked.Count > 0)
            {
                _tracked.RemoveAll(e => e == null);
                yield return null;
            }
        }

        [Header("Boss")]
        [SerializeField] private Enemy.EnemyData _bossMinionData;

        [Header("Ranged Shooter")]
        [SerializeField] private GameObject _enemyProjectilePrefab;

        /// <summary>
        /// External entry point for on-death splitters or boss minions to
        /// register spawned enemies into the wave's tracked list.
        /// </summary>
        public void SpawnExtra(Enemy.EnemyData data, int count, Vector3 originPos)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(
                    UnityEngine.Random.Range(-0.6f, 0.6f),
                    UnityEngine.Random.Range(-0.4f, 0.4f)
                );
                SpawnAt(data, 1f, (Vector2)originPos + offset);
            }
        }

        private void SpawnAt(Enemy.EnemyData data, float speedMul, Vector2 pos)
        {
            if (_enemyPrefab == null) return;
            var go = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            if (go.TryGetComponent<Enemy.Enemy>(out var enemy))
            {
                if (data != null) enemy.Configure(data);
                _tracked.Add(enemy);
                enemy.OnDeath += HandleEnemyDeath;
            }
            if (go.TryGetComponent<Enemy.EnemyMover>(out var mover))
                mover.SetSpeedMultiplier(speedMul <= 0f ? 1f : speedMul);
            AttachOptionalBehaviors(go, data);
        }

        private void AttachOptionalBehaviors(GameObject go, Enemy.EnemyData data)
        {
            if (data == null) return;
            if (data.isBoss)
            {
                var boss = go.AddComponent<Enemy.BossBehavior>();
                boss.Configure(_enemyPrefab, _bossMinionData != null ? _bossMinionData : data);
            }
            if (data.isRangedShooter)
            {
                var shooter = go.AddComponent<Enemy.RangedShooterBehavior>();
                shooter.Configure(_enemyProjectilePrefab, data);
            }
        }

        private void SpawnOne(Enemy.EnemyData data, float speedMul)
        {
            Vector2 pos = new Vector2(
                UnityEngine.Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                UnityEngine.Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
            );
            SpawnAt(data, speedMul, pos);
        }

        private void HandleEnemyDeath(Enemy.Enemy e)
        {
            _tracked.Remove(e);
        }

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                var tmp = list[k]; list[k] = list[n]; list[n] = tmp;
            }
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
