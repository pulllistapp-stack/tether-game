using UnityEngine;

namespace Tether.Enemy
{
    public enum EnemyBehavior
    {
        FallHoming,   // baseline: drift down + horizontal homing
        FallStraight, // no homing (heavy tank)
        Sidewind,     // sine-wave horizontal weave while falling
        FloatSteady,  // hovers around _floatY, bobs slowly (boss)
    }

    /// <summary>
    /// Phase 3 ScriptableObject-driven enemy config. WaveSystem spawns the
    /// generic Enemy prefab and calls Configure() with the chosen data.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Tether/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Grunt";
        public Color tintColor = new Color(0.9f, 0.25f, 0.35f, 1f);
        public float sizeMultiplier = 1f;
        [Tooltip("Optional art override. When set, Enemy.Configure swaps the SpriteRenderer to this and forces the tint to white so the art reads true.")]
        public Sprite spriteOverride;

        [Header("Stats")]
        public float maxHp = 5f;

        [Header("Movement")]
        public EnemyBehavior behavior = EnemyBehavior.FallStraight;
        public float fallSpeed = 0.6f;
        [Range(0f, 1f)] public float homingStrength = 0.25f;
        [Tooltip("Sidewind only: horizontal amplitude in world units.")]
        public float sineAmplitude = 1.5f;
        [Tooltip("Sidewind only: oscillations per second.")]
        public float sineFrequency = 0.6f;

        [Header("FloatSteady (boss)")]
        [Tooltip("Y position the boss hovers around.")]
        public float floatY = 3.5f;
        [Tooltip("Vertical bob amplitude.")]
        public float floatBobAmp = 0.6f;
        [Tooltip("Horizontal drift amplitude around center X.")]
        public float floatDriftAmp = 2.5f;

        [Header("Boss")]
        [Tooltip("Marks this data as the run's boss — WaveSystem attaches BossBehavior at spawn.")]
        public bool isBoss = false;

        [Header("Rewards")]
        [Tooltip("XP value the dropped orb carries.")]
        public int xpReward = 1;
        [Tooltip("Coin drop count (each coin is worth 1).")]
        public int coinReward = 1;

        [Header("Ranged Shooter")]
        [Tooltip("Marks this data as a shooter — WaveSystem attaches RangedShooterBehavior at spawn.")]
        public bool isRangedShooter = false;
        public float shootInterval = 2.2f;
        public float shootStartDelay = 1.4f;
        public float projectileSpeed = 5f;
        public float projectileDamage = 1f;

        [Header("Splitter (on-death behavior)")]
        [Tooltip("If true, spawn N of splitInto on death.")]
        public bool isSplitter = false;
        public EnemyData splitInto;
        public int splitCount = 2;

        [Header("Teleporter")]
        [Tooltip("If true, blinks to a new random X position on cadence.")]
        public bool isTeleporter = false;
        public float teleportInterval = 2.5f;
        public float teleportRangeX = 4f;

        [Header("Healer")]
        [Tooltip("If true, periodically heals nearby enemies within radius.")]
        public bool isHealer = false;
        public float healInterval = 3f;
        public float healRadius = 2.5f;
        public float healAmount = 2f;
    }
}
