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

        [Header("Stats")]
        public float maxHp = 5f;

        [Header("Movement")]
        public EnemyBehavior behavior = EnemyBehavior.FallHoming;
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
    }
}
