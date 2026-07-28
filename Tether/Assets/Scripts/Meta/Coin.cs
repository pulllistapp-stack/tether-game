using UnityEngine;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 4 coin pickup. Magnetically drifts toward the player when within
    /// _magnetRadius; delivered when within _pickupRadius. No physics.
    /// </summary>
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int _value = 1;
        [SerializeField] private float _magnetRadius = 4f;
        [SerializeField] private float _pickupRadius = 0.4f;
        [SerializeField] private float _seekSpeed = 8f;
        [SerializeField] private float _idleDrift = 1.5f; // fall gently until magnet catches
        [SerializeField] private float _lifetime = 12f;

        private Transform _player;
        private Vector2 _idleVel;
        private float _spawnTime;

        /// <summary>Instantly grants this coin's value, bypassing magnet range/distance.
        /// Called on wave-clear so leftover coins outside pickup range aren't wasted.</summary>
        public void ForceCollect()
        {
            CoinWallet.Instance?.AddCoin(_value);
            Utility.Fx.Sparkle(transform.position, new Color(1f, 0.85f, 0.25f), 5, 3.2f);
            Destroy(gameObject);
        }

        private void Awake()
        {
            _spawnTime = Time.time;
            _idleVel = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 1.5f));

            // Pickup lights removed — user asked for lights on flying projectiles only.
        }

        private void Start()
        {
            var pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo != null) _player = pgo.transform;
        }

        private void Update()
        {
            if (Time.time - _spawnTime > _lifetime) { Destroy(gameObject); return; }
            if (_player == null) return;

            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist <= _pickupRadius)
            {
                CoinWallet.Instance?.AddCoin(_value);
                Utility.Fx.Sparkle(transform.position, new Color(1f, 0.85f, 0.25f), 5, 3.2f);
                Destroy(gameObject);
                return;
            }

            if (dist <= _magnetRadius)
            {
                // seek player, easing speed by proximity
                Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                float easedSpeed = Mathf.Lerp(_seekSpeed * 0.4f, _seekSpeed, 1f - dist / _magnetRadius);
                transform.position += (Vector3)(dir * easedSpeed * Time.deltaTime);
            }
            else
            {
                // gentle drift down + damping
                _idleVel = Vector2.Lerp(_idleVel, new Vector2(0f, -_idleDrift), Time.deltaTime * 1.5f);
                transform.position += (Vector3)(_idleVel * Time.deltaTime);
            }
        }
    }
}
