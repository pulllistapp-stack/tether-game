using UnityEngine;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 7C. On arena scene load, reads RunSession.CurrentNode.type and
    /// swaps WaveSystem's wave list to that node's payload before WaveSystem
    /// starts running. Sits in the arena scene; Awake runs before Start.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class NodeConfigApplier : MonoBehaviour
    {
        [SerializeField] private WaveSystem _waveSystem;

        private void Awake()
        {
            if (_waveSystem == null) _waveSystem = FindFirstObjectByType<WaveSystem>();
            if (_waveSystem == null) return;

            var session = Meta.RunSession.Instance;
            if (session == null || session.CurrentNode == null) return;

            var allWaves = _waveSystem.AllWaves;
            if (allWaves == null || allWaves.Length == 0) return;

            int pickIndex = MapNodeToWaveIndex(session.CurrentNode.type, allWaves.Length);
            if (pickIndex < 0 || pickIndex >= allWaves.Length) return;

            _waveSystem.OverrideWaves(new WaveSystem.WaveDefinition[] { allWaves[pickIndex] });
        }

        private int MapNodeToWaveIndex(Meta.NodeType type, int totalWaves)
        {
            // Fallback: default 4-wave layout is [Combat, Combat+, Elite mix, Boss]
            switch (type)
            {
                case Meta.NodeType.Combat: return 0;
                case Meta.NodeType.Elite:  return Mathf.Min(2, totalWaves - 2);
                case Meta.NodeType.Boss:   return totalWaves - 1;
                default:                   return 0;
            }
        }
    }
}
