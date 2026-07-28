using UnityEngine;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 7 Relic. Distinct from an UpgradeCard: relics are named,
    /// unique-per-run permanent effects that hook into specific systems
    /// (Time Stop charge rate, on-kill heal, etc). RelicSystem.HasRelic(id)
    /// gates the effect at the callsite.
    /// </summary>
    [CreateAssetMenu(fileName = "Relic", menuName = "Tether/Relic")]
    public class Relic : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable id used by RelicSystem.HasRelic() — never change after ship.")]
        public string id = "relic_id";
        public string displayName = "Untitled Relic";
        [TextArea(2, 4)]
        public string description = "What it does.";
        [TextArea(1, 2)]
        public string flavor = "";
        public Color tint = new Color(1f, 0.85f, 0.5f, 1f);

        [Header("Rarity")]
        [Range(1, 10)] public int weight = 5;
    }
}
