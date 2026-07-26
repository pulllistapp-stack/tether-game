using UnityEngine;

namespace Tether.Meta
{
    public enum ShopEffect
    {
        UnlockBall,       // adds a BallData to the player's ball roster
        StartingHpBonus,  // +N max HP at run start
        StartingCoins,    // begin the run with N coins
        FasterNova,       // reduce nova cooldown by seconds
    }

    /// <summary>
    /// Phase 5 shop item. One-time unlock, referenced by string id.
    /// RunStartup reads all unlocked ShopItems and applies their effects.
    /// </summary>
    [CreateAssetMenu(fileName = "ShopItem", menuName = "Tether/Shop Item")]
    public class ShopItem : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable id used in PlayerPrefs — never change it after ship.")]
        public string id = "item_id";
        public string displayName = "Item";
        [TextArea(2, 4)]
        public string description = "What it does.";
        public Color tint = Color.white;

        [Header("Economy")]
        public int cost = 100;

        [Header("Effect")]
        public ShopEffect effect = ShopEffect.UnlockBall;
        public float magnitude = 1f;
        [Tooltip("Ball data for UnlockBall effect.")]
        public Gameplay.BallData ballDataUnlock;
    }
}
