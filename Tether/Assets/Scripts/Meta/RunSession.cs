using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tether.Meta
{
    public enum NodeType
    {
        Combat,
        Elite,
        Rest,
        Shop,
        Boss,
    }

    [Serializable]
    public class MapNode
    {
        public NodeType type;
        public int row;
        public bool cleared;
    }

    /// <summary>
    /// Phase 7C run session. Persists across MainMenu → Map → Arena → Map
    /// scene loads so relics/coins/HP survive between nodes. Cleared on
    /// ResetForNewRun (called by MainMenu.StartRun).
    /// </summary>
    public class RunSession : MonoBehaviour
    {
        public static RunSession Instance { get; private set; }

        public List<MapNode> Map { get; private set; } = new List<MapNode>();
        public int CurrentNodeIndex { get; private set; } = -1;
        public MapNode CurrentNode =>
            (CurrentNodeIndex >= 0 && CurrentNodeIndex < Map.Count) ? Map[CurrentNodeIndex] : null;

        public int CachedPlayerHp { get; private set; } = -1;
        public int CachedPlayerMaxHp { get; private set; } = -1;
        public bool RunActive { get; private set; }

        public event Action OnMapReset;
        public event Action OnNodeCleared;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ResetForNewRun()
        {
            RunActive = true;
            CachedPlayerHp = -1;
            CachedPlayerMaxHp = -1;
            CurrentNodeIndex = -1;

            // Wipe other per-run singletons if they're around
            var applier = FindFirstObjectByType<Systems.UpgradeApplier>();
            if (applier != null) Destroy(applier.gameObject);
            var relics = FindFirstObjectByType<Systems.RelicSystem>();
            if (relics != null) Destroy(relics.gameObject);
            var wallet = FindFirstObjectByType<CoinWallet>();
            if (wallet != null) Destroy(wallet.gameObject);
            var level = FindFirstObjectByType<LevelSystem>();
            if (level != null) Destroy(level.gameObject);
            var stats = FindFirstObjectByType<RunStats>();
            if (stats != null) Destroy(stats.gameObject);

            GenerateMap();
            OnMapReset?.Invoke();
        }

        /// <summary>Simple 5-node linear map: 2 Combat → Elite → Rest → Boss.</summary>
        public void GenerateMap()
        {
            Map.Clear();
            Map.Add(new MapNode { type = NodeType.Combat, row = 0 });
            Map.Add(new MapNode { type = NodeType.Combat, row = 1 });
            Map.Add(new MapNode { type = NodeType.Elite,  row = 2 });
            Map.Add(new MapNode { type = NodeType.Rest,   row = 3 });
            Map.Add(new MapNode { type = NodeType.Boss,   row = 4 });
        }

        public void SetCurrentNode(int index)
        {
            if (index < 0 || index >= Map.Count) return;
            CurrentNodeIndex = index;
        }

        public bool IsNodeAvailable(int index)
        {
            if (index < 0 || index >= Map.Count) return false;
            if (Map[index].cleared) return false;
            // Available if previous node is cleared (or this is node 0)
            return index == 0 || Map[index - 1].cleared;
        }

        public void MarkCurrentCleared()
        {
            if (CurrentNode == null) return;
            CurrentNode.cleared = true;
            OnNodeCleared?.Invoke();
        }

        public bool AllCleared()
        {
            foreach (var n in Map) if (!n.cleared) return false;
            return true;
        }

        public void CachePlayerHp(int cur, int max)
        {
            CachedPlayerHp = cur;
            CachedPlayerMaxHp = max;
        }

        public void EndRun()
        {
            RunActive = false;
        }
    }
}
