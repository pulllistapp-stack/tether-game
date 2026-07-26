using UnityEngine;

namespace Tether.Systems
{
    public enum UpgradeEffect
    {
        BallDamage,          // multiplier
        BallSpeed,           // multiplier
        BallSize,            // multiplier
        FireCooldown,        // multiplier (< 1 = faster)
        ExtraProjectile,     // additive int (each fire spawns N extra)
        PlayerMaxHp,         // additive int
        PlayerHeal,          // additive int, applied immediately
        ExplosionRadius,     // multiplier (Explosive only)
        SplitCount,          // additive int (Split only)
        SplitDepth,          // additive int (Split only)
        BounceExtension,     // additive int (adds to ball _returnAfterBounces)
        CoinPerKill,         // additive int (bonus coins per enemy death)
        XpPerKill,           // additive int (bonus XP per enemy death)
    }

    /// <summary>
    /// Phase 3 roguelite upgrade card. Data + description + magnitude.
    /// UpgradeApplier interprets Effect + Magnitude to mutate live state.
    /// </summary>
    [CreateAssetMenu(fileName = "UpgradeCard", menuName = "Tether/Upgrade Card")]
    public class UpgradeCard : ScriptableObject
    {
        [Header("Display")]
        public string displayName = "Untitled";
        [TextArea(2, 4)]
        public string description = "Effect description.";
        public Color tint = Color.white;
        public Sprite icon;

        [Header("Effect")]
        public UpgradeEffect effect = UpgradeEffect.BallDamage;
        [Tooltip("For multipliers: 1.25 = +25%. For additives: whole number (e.g. 2 HP).")]
        public float magnitude = 1.25f;

        [Header("Rarity")]
        [Range(1, 10)] public int weight = 5;
    }
}
