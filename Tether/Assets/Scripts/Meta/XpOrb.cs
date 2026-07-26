using UnityEngine;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 5 XP orb. Same shape as Coin (drift + magnet-seek); on pickup
    /// delivers XP to the LevelSystem instead of coins.
    /// </summary>
    public class XpOrb : MonoBehaviour
    {
        [SerializeField] private int _value = 1;
        [SerializeField] private float _magnetRadius = 5f;
        [SerializeField] private float _pickupRadius = 0.4f;
        [SerializeField] private float _seekSpeed = 9f;
        [SerializeField] private float _idleDrift = 1.2f;
        [SerializeField] private float _lifetime = 15f;

        private Transform _player;
        private Vector2 _idleVel;
        private float _spawnTime;

        public void SetValue(int v) => _value = v;

        private void Awake()
        {
            _spawnTime = Time.time;
            _idleVel = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 1.5f));
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
                LevelSystem.Instance?.AddXp(_value);
                Destroy(gameObject);
                return;
            }

            if (dist <= _magnetRadius)
            {
                Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                float easedSpeed = Mathf.Lerp(_seekSpeed * 0.5f, _seekSpeed, 1f - dist / _magnetRadius);
                transform.position += (Vector3)(dir * easedSpeed * Time.deltaTime);
            }
            else
            {
                _idleVel = Vector2.Lerp(_idleVel, new Vector2(0f, -_idleDrift), Time.deltaTime * 1.5f);
                transform.position += (Vector3)(_idleVel * Time.deltaTime);
            }
        }
    }
}
