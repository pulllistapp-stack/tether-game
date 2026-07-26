using UnityEngine;
using System;
using System.Collections;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 7 Sakuya signature. Charges as balls bounce and hit enemies. When
    /// full, F triggers a global freeze on enemies + enemy projectiles for
    /// _duration seconds. Balls and Player keep moving — this is offensive
    /// tempo control, not a full pause.
    /// </summary>
    public class TimeStopSystem : MonoBehaviour
    {
        public static TimeStopSystem Instance { get; private set; }

        [Header("Charge")]
        [SerializeField] private float _maxCharge = 100f;
        [SerializeField] private float _chargePerWallBounce = 3f;
        [SerializeField] private float _chargePerEnemyHit = 8f;
        [SerializeField] private float _chargePerEnemyKill = 15f;

        [Header("Freeze")]
        [SerializeField] private KeyCode _activateKey = KeyCode.F;
        [SerializeField] private float _duration = 3f;

        public float Charge { get; private set; }
        public float MaxCharge => _maxCharge;
        public float Charge01 => Mathf.Clamp01(Charge / _maxCharge);
        public bool IsReady => Charge >= _maxCharge;
        public bool IsActive { get; private set; }
        public float ActiveTimeRemaining { get; private set; }

        public event Action OnActivated;
        public event Action OnDeactivated;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_activateKey) && IsReady && !IsActive)
            {
                StartCoroutine(FreezeRoutine());
            }
        }

        public void AddCharge(float amount)
        {
            if (IsActive) return; // no charge accumulation during freeze
            // Sand Hourglass relic doubles Sakuya's charge rate
            if (RelicSystem.Instance != null && RelicSystem.Instance.HasRelic("sand_hourglass"))
                amount *= 2f;
            Charge = Mathf.Min(_maxCharge, Charge + amount);
        }

        public void AddChargeFromWallBounce() => AddCharge(_chargePerWallBounce);
        public void AddChargeFromEnemyHit()   => AddCharge(_chargePerEnemyHit);
        public void AddChargeFromEnemyKill()  => AddCharge(_chargePerEnemyKill);

        private IEnumerator FreezeRoutine()
        {
            IsActive = true;
            ActiveTimeRemaining = _duration;
            OnActivated?.Invoke();

            Audio.AudioManager.Instance?.Play("wave_start", 0.7f, 0.02f);
            if (CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(0.15f);

            while (ActiveTimeRemaining > 0f)
            {
                ActiveTimeRemaining -= Time.unscaledDeltaTime;
                yield return null;
            }

            IsActive = false;
            Charge = 0f;
            OnDeactivated?.Invoke();
            Audio.AudioManager.Instance?.Play("ball_bounce", 0.5f, 0.02f);
        }
    }
}
