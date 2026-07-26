using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 7 Relic wallet. Tracks which relics are active for THIS run.
    /// Gameplay code checks HasRelic(id) at hot loop callsites — the effect
    /// hooks live where they belong (TimeStopSystem, Enemy.Die, Ball, ...).
    /// </summary>
    public class RelicSystem : MonoBehaviour
    {
        public static RelicSystem Instance { get; private set; }

        private readonly HashSet<string> _owned = new HashSet<string>();
        private readonly List<Relic> _list = new List<Relic>();
        public IReadOnlyList<Relic> Owned => _list;

        public event Action<Relic> OnRelicGranted;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool HasRelic(string id) => _owned.Contains(id);

        public void Grant(Relic relic)
        {
            if (relic == null) return;
            if (_owned.Contains(relic.id)) return;
            _owned.Add(relic.id);
            _list.Add(relic);
            OnRelicGranted?.Invoke(relic);
        }
    }
}
