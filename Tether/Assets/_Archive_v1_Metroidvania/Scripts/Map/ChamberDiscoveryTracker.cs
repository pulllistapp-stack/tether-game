using System.Collections.Generic;
using UnityEngine;
using Tether.Save;

namespace Tether.Map
{
    public class ChamberDiscoveryTracker : MonoBehaviour
    {
        public static ChamberDiscoveryTracker Instance { get; private set; }

        [SerializeField] private List<ChamberData> _allChambers = new List<ChamberData>();

        public ChamberData CurrentChamber { get; private set; }
        public System.Action<ChamberData> OnChamberEntered;
        public System.Action<ChamberData> OnChamberDiscovered;

        private HashSet<string> _discovered = new HashSet<string>();
        private Dictionary<string, ChamberData> _byId = new Dictionary<string, ChamberData>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var c in _allChambers)
            {
                if (c != null && !string.IsNullOrEmpty(c.ChamberId))
                    _byId[c.ChamberId] = c;
            }

            foreach (var id in SaveSystem.Current.DiscoveredChambers)
                _discovered.Add(id);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool IsDiscovered(string chamberId) => _discovered.Contains(chamberId);

        public IReadOnlyCollection<string> Discovered => _discovered;
        public IReadOnlyList<ChamberData> AllChambers => _allChambers;

        public void EnterChamber(string chamberId)
        {
            if (!_byId.TryGetValue(chamberId, out var data)) return;
            CurrentChamber = data;
            SaveSystem.Current.CurrentChamberId = chamberId;
            SaveSystem.Current.CurrentFloor = data.Floor;

            bool firstTime = !_discovered.Contains(chamberId);
            if (firstTime)
            {
                _discovered.Add(chamberId);
                SaveSystem.Current.DiscoveredChambers.Add(chamberId);
                SaveSystem.Save();
                OnChamberDiscovered?.Invoke(data);
            }

            OnChamberEntered?.Invoke(data);
        }

        public IEnumerable<ChamberData> ChambersOnFloor(int floor)
        {
            foreach (var c in _allChambers)
                if (c != null && c.Floor == floor) yield return c;
        }
    }
}
