using UnityEngine;
using System.Collections;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 5 ranged shooter layer. Periodically fires a straight projectile
    /// aimed at the player. Attached at spawn when EnemyData.isRangedShooter.
    /// </summary>
    public class RangedShooterBehavior : MonoBehaviour
    {
        [SerializeField] private float _interval = 2.2f;
        [SerializeField] private float _startDelay = 1.4f;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private float _projectileDamage = 1f;
        [SerializeField] private GameObject _projectilePrefab;

        private Coroutine _routine;

        public void Configure(GameObject projectilePrefab, EnemyData data)
        {
            _projectilePrefab = projectilePrefab;
            if (data != null)
            {
                _interval = data.shootInterval;
                _startDelay = data.shootStartDelay;
                _projectileSpeed = data.projectileSpeed;
                _projectileDamage = data.projectileDamage;
            }
        }

        private void Start()
        {
            _routine = StartCoroutine(FireLoop());
        }

        private IEnumerator FireLoop()
        {
            yield return new WaitForSeconds(_startDelay);
            while (true)
            {
                Fire();
                yield return new WaitForSeconds(_interval);
            }
        }

        private void Fire()
        {
            if (_projectilePrefab == null) return;
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO == null) return;

            Vector2 origin = transform.position;
            Vector2 dir = ((Vector2)playerGO.transform.position - origin).normalized;

            var proj = Instantiate(_projectilePrefab, origin + dir * 0.6f, Quaternion.identity);
            if (proj.TryGetComponent<EnemyProjectile>(out var ep))
            {
                ep.Launch(dir, _projectileSpeed, _projectileDamage);
            }

            Audio.AudioManager.Instance?.Play("ball_fire", 0.5f, 0.02f);
        }
    }
}
