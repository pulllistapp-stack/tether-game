using UnityEngine;
using System.Collections;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 1 baseline WaveSpawner. Spawns N enemies within a rectangular area.
    /// Extended later with wave patterns, difficulty scaling, boss triggers.
    /// </summary>
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Spawn Config")]
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private int _enemyCount = 5;

        [Header("Spawn Area (world coordinates)")]
        [SerializeField] private Vector2 _spawnAreaMin = new Vector2(-3f, 2f);
        [SerializeField] private Vector2 _spawnAreaMax = new Vector2(3f, 4f);

        [Header("Timing")]
        [Tooltip("Delay before spawning starts (seconds).")]
        [SerializeField] private float _startDelay = 1f;
        [Tooltip("Interval between individual enemy spawns (0 = all at once).")]
        [SerializeField] private float _spawnInterval = 0.2f;

        private void Start()
        {
            StartCoroutine(SpawnWaveRoutine());
        }

        private IEnumerator SpawnWaveRoutine()
        {
            yield return new WaitForSeconds(_startDelay);

            for (int i = 0; i < _enemyCount; i++)
            {
                SpawnOne();
                if (_spawnInterval > 0f)
                {
                    yield return new WaitForSeconds(_spawnInterval);
                }
            }
        }

        private void SpawnOne()
        {
            if (_enemyPrefab == null) return;

            Vector2 pos = new Vector2(
                Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
            );
            Instantiate(_enemyPrefab, pos, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (_spawnAreaMin.x + _spawnAreaMax.x) * 0.5f,
                (_spawnAreaMin.y + _spawnAreaMax.y) * 0.5f,
                0f
            );
            Vector3 size = new Vector3(
                _spawnAreaMax.x - _spawnAreaMin.x,
                _spawnAreaMax.y - _spawnAreaMin.y,
                0f
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}
