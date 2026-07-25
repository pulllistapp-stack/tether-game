using UnityEngine;
using System.Collections;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 4 boss layer. Added on top of a regular Enemy + EnemyMover
    /// when the spawn's EnemyData.isBoss is set. Periodically spawns a
    /// minion (grunt-style small enemy). No health of its own — Enemy.cs
    /// still handles HP + death.
    /// </summary>
    public class BossBehavior : MonoBehaviour
    {
        [SerializeField] private float _minionInterval = 5f;
        [SerializeField] private float _startDelay = 3f;
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private EnemyData _minionData;

        private Coroutine _routine;

        public void Configure(GameObject enemyPrefab, EnemyData minionData)
        {
            _enemyPrefab = enemyPrefab;
            _minionData = minionData;
        }

        private void Start()
        {
            _routine = StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(_startDelay);
            while (true)
            {
                SpawnOne();
                yield return new WaitForSeconds(_minionInterval);
            }
        }

        private void SpawnOne()
        {
            if (_enemyPrefab == null) return;

            Vector2 pos = (Vector2)transform.position + new Vector2(Random.Range(-1.5f, 1.5f), -1.2f);
            var go = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            if (go.TryGetComponent<Enemy>(out var e) && _minionData != null) e.Configure(_minionData);
            if (go.TryGetComponent<EnemyMover>(out var m)) m.SetSpeedMultiplier(1.2f);
        }
    }
}
