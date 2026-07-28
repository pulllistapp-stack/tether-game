using UnityEngine;
using System.Collections.Generic;

namespace Tether.Audio
{
    /// <summary>
    /// Phase 3 audio bus. Generates a library of 8-bit style SFX at boot and
    /// plays them by ID via a pooled AudioSource fleet. Also runs a simple
    /// procedural BGM loop.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Volumes")]
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 0.7f;
        [Range(0f, 1f)] [SerializeField] private float _bgmVolume = 0.28f;
        [SerializeField] private int _sfxSourcePoolSize = 8;
        [SerializeField] private bool _playBgm = true;

        private readonly Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
        private AudioSource[] _sfxSources;
        private int _sfxIndex;
        private AudioSource _bgmSource;
        private float _lastPlayedAt;
        private readonly Dictionary<string, float> _lastPlayedById = new Dictionary<string, float>();
        private const float MinPerIdInterval = 0.015f; // basic anti-machine-gun

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            _sfxSources = new AudioSource[_sfxSourcePoolSize];
            for (int i = 0; i < _sfxSourcePoolSize; i++)
            {
                var go = new GameObject("SFX_" + i);
                go.transform.SetParent(transform, false);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                _sfxSources[i] = src;
            }

            var bgmGO = new GameObject("BGM");
            bgmGO.transform.SetParent(transform, false);
            _bgmSource = bgmGO.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            _bgmSource.spatialBlend = 0f;
            _bgmSource.volume = _bgmVolume;

            GenerateLibrary();

            if (_playBgm && _clips.TryGetValue("bgm_loop", out var bgm))
            {
                _bgmSource.clip = bgm;
                _bgmSource.Play();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            UnsubscribeGameplay();
        }

        private Systems.WaveSystem _wave;
        private Systems.UpgradeSelector _selector;
        private Player.PlayerHealth _playerHp;

        private void Start()
        {
            SubscribeGameplay();
        }

        private void SubscribeGameplay()
        {
            _wave = FindFirstObjectByType<Systems.WaveSystem>();
            if (_wave != null)
            {
                _wave.OnWaveStarted += HandleWaveStarted;
                _wave.OnWaveCleared += HandleWaveCleared;
            }
            _selector = FindFirstObjectByType<Systems.UpgradeSelector>();
            if (_selector != null) _selector.OnPicked += HandleUpgradePicked;

            _playerHp = FindFirstObjectByType<Player.PlayerHealth>();
            if (_playerHp != null)
            {
                _playerHp.OnDamaged += HandlePlayerDamaged;
                _playerHp.OnDied += HandlePlayerDied;
            }
        }

        private void UnsubscribeGameplay()
        {
            if (_wave != null)
            {
                _wave.OnWaveStarted -= HandleWaveStarted;
                _wave.OnWaveCleared -= HandleWaveCleared;
            }
            if (_selector != null) _selector.OnPicked -= HandleUpgradePicked;
            if (_playerHp != null)
            {
                _playerHp.OnDamaged -= HandlePlayerDamaged;
                _playerHp.OnDied -= HandlePlayerDied;
            }
        }

        private void HandleWaveStarted(int idx, int total) => Play("wave_start");
        private void HandleWaveCleared(int idx) { if (_wave != null && idx < _wave.TotalWaves - 1) Play("wave_clear"); }
        private void HandleUpgradePicked() => Play("upgrade_pick");
        private int _lastHp = -1;
        private void HandlePlayerDamaged(int cur, int max)
        {
            if (_lastHp < 0) { _lastHp = cur; return; }
            if (cur < _lastHp) Play("player_hit");
            _lastHp = cur;
        }
        private void HandlePlayerDied() => Play("player_die");

        public void Play(string id, float volumeScale = 1f, float pitchJitter = 0.05f)
        {
            if (!_clips.TryGetValue(id, out var clip)) return;
            float now = Time.unscaledTime;
            if (_lastPlayedById.TryGetValue(id, out var last) && now - last < MinPerIdInterval) return;
            _lastPlayedById[id] = now;

            var src = _sfxSources[_sfxIndex];
            _sfxIndex = (_sfxIndex + 1) % _sfxSources.Length;
            src.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
            src.volume = _sfxVolume * volumeScale;
            src.PlayOneShot(clip);
        }

        // ----- Library -----

        private void GenerateLibrary()
        {
            // Ball fire: quick chirp up, sine, short
            Register("ball_fire", ProceduralAudio.Tone(
                "ball_fire", 340f, 720f, 0.08f, WaveType.Square, 0.45f, 2.5f));

            // Ball wall bounce: low soft blip
            Register("ball_bounce", ProceduralAudio.Tone(
                "ball_bounce", 220f, 180f, 0.05f, WaveType.Sine, 0.35f, 2.0f));

            // Ball hit enemy: noise crack + tone
            var noise = ProceduralAudio.Tone("_h_n", 800f, 800f, 0.05f, WaveType.Noise, 0.5f, 2f);
            var tone  = ProceduralAudio.Tone("_h_t", 520f, 260f, 0.09f, WaveType.Square, 0.5f, 2.5f);
            Register("ball_hit_enemy", ProceduralAudio.Mix("ball_hit_enemy", noise, tone, 0.6f, 1f));

            // Enemy die: descending sweep, saw
            Register("enemy_die", ProceduralAudio.Tone(
                "enemy_die", 520f, 90f, 0.22f, WaveType.Saw, 0.55f, 1.7f));

            // Player hit: gritty noise burst
            var pHitNoise = ProceduralAudio.Tone("_ph_n", 400f, 200f, 0.14f, WaveType.Noise, 0.65f, 1.5f);
            var pHitTone  = ProceduralAudio.Tone("_ph_t", 260f, 120f, 0.14f, WaveType.Triangle, 0.5f, 1.5f);
            Register("player_hit", ProceduralAudio.Mix("player_hit", pHitNoise, pHitTone, 0.6f, 1f));

            // Player die: long descending sweep
            Register("player_die", ProceduralAudio.Tone(
                "player_die", 600f, 60f, 0.7f, WaveType.Saw, 0.65f, 1.5f));

            // Wave start: rising two-tone stinger
            var wsA = ProceduralAudio.Tone("_ws_a", 440f, 440f, 0.10f, WaveType.Square, 0.45f, 1.5f);
            var wsB = ProceduralAudio.Tone("_ws_b", 660f, 660f, 0.14f, WaveType.Square, 0.45f, 1.5f);
            Register("wave_start", ProceduralAudio.Sequence("wave_start", wsA, wsB));

            // Wave clear: 3-note ascending stinger
            var wcA = ProceduralAudio.Tone("_wc_a", 523f, 523f, 0.10f, WaveType.Triangle, 0.5f, 1.4f); // C5
            var wcB = ProceduralAudio.Tone("_wc_b", 659f, 659f, 0.10f, WaveType.Triangle, 0.5f, 1.4f); // E5
            var wcC = ProceduralAudio.Tone("_wc_c", 784f, 784f, 0.25f, WaveType.Triangle, 0.5f, 1.4f); // G5
            Register("wave_clear", ProceduralAudio.Sequence("wave_clear", wcA, wcB, wcC));

            // Upgrade pick: sparkle up
            var upA = ProceduralAudio.Tone("_up_a", 660f, 990f, 0.10f, WaveType.Sine, 0.5f, 1.8f);
            var upB = ProceduralAudio.Tone("_up_b", 990f, 1320f, 0.14f, WaveType.Sine, 0.45f, 1.8f);
            Register("upgrade_pick", ProceduralAudio.Sequence("upgrade_pick", upA, upB));

            // BGM: 4-note tonic-drone loop, slow, low volume
            var b1 = ProceduralAudio.Tone("_b1", 130.8f, 130.8f, 0.5f, WaveType.Triangle, 0.4f, 0.8f, 0.05f); // C3
            var b2 = ProceduralAudio.Tone("_b2", 155.6f, 155.6f, 0.5f, WaveType.Triangle, 0.4f, 0.8f, 0.05f); // Eb3
            var b3 = ProceduralAudio.Tone("_b3", 174.6f, 174.6f, 0.5f, WaveType.Triangle, 0.4f, 0.8f, 0.05f); // F3
            var b4 = ProceduralAudio.Tone("_b4", 130.8f, 130.8f, 0.5f, WaveType.Triangle, 0.4f, 0.8f, 0.05f);
            Register("bgm_loop", ProceduralAudio.Sequence("bgm_loop", b1, b2, b3, b2, b1, b3, b4, b2));
        }

        private void Register(string id, AudioClip clip) => _clips[id] = clip;
    }
}
