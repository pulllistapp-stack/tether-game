using UnityEngine;

namespace Tether.Gameplay
{
    public enum BallBehavior
    {
        Normal,
        Split,
        Explosive,
        Piercing,
        Homing,
        Lightning,
        Freeze,
    }

    /// <summary>
    /// Phase 2 ScriptableObject-driven ball config. One asset per ball type;
    /// Ball.Configure() applies it at spawn. Enables slot swap without new prefabs.
    /// </summary>
    [CreateAssetMenu(fileName = "BallData", menuName = "Tether/Ball Data")]
    public class BallData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Normal";
        public Color tintColor = new Color(1f, 0.85f, 0.3f, 1f);
        public Sprite icon;

        [Header("Physics")]
        public float speed = 12f;
        public float damage = 1f;
        [Tooltip("-1 = infinite bounces before self-destruct.")]
        public int maxBounces = -1;
        [Tooltip("Local visual scale multiplier.")]
        public float sizeMultiplier = 1f;

        [Header("Behavior")]
        public BallBehavior behavior = BallBehavior.Normal;

        [Header("Split (Split-behavior only)")]
        [Tooltip("Number of child balls spawned when this ball hits a wall.")]
        public int splitCount = 2;
        [Tooltip("Splay angle for each side of the child spawn (degrees).")]
        public float splitAngle = 30f;
        [Tooltip("Which BallData the split children use (null = destroy on split).")]
        public BallData splitChild;
        [Tooltip("Max chained split depth before children stop splitting.")]
        public int splitMaxDepth = 1;

        [Header("Explosive (Explosive-behavior only)")]
        [Tooltip("Radius of the on-hit AoE damage circle.")]
        public float explosionRadius = 1.2f;
        [Tooltip("Damage dealt to enemies inside the radius (in addition to contact damage).")]
        public float explosionDamage = 1f;

        [Header("Homing (Homing-behavior only)")]
        [Tooltip("How aggressively the ball turns toward the nearest enemy (deg/sec).")]
        public float homingTurnRate = 220f;
        [Tooltip("Max range for homing detection (world units).")]
        public float homingRange = 6f;

        [Header("Lightning (Lightning-behavior only)")]
        [Tooltip("Number of additional enemies zapped when a Lightning ball hits an enemy.")]
        public int chainCount = 3;
        [Tooltip("Search radius for chain targets (world units).")]
        public float chainRange = 3.5f;
        [Tooltip("Damage applied to each chained enemy (before upgrade multipliers).")]
        public float chainDamage = 1f;

        [Header("Freeze (Freeze-behavior only)")]
        [Tooltip("How long the hit enemy is slowed (seconds).")]
        public float freezeDuration = 2.5f;
        [Tooltip("Speed multiplier applied while slowed (0.2 = 20% speed).")]
        public float freezeSlowFactor = 0.15f;
    }
}
