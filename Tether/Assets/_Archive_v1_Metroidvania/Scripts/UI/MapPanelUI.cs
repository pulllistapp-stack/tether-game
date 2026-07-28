using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tether.Map;
using Tether.Save;

namespace Tether.UI
{
    public class MapPanelUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform _floorContainer;
        [SerializeField] private GameObject _floorRowPrefab;
        [SerializeField] private GameObject _chamberSlotPrefab;
        [SerializeField] private Text _currentFloorLabel;
        [SerializeField] private Text _discoveryCountLabel;

        [Header("Settings")]
        [SerializeField, Range(1, 10)] private int _floorCount = 7;
        [SerializeField] private Color _undiscoveredColor = new Color(0.1f, 0.1f, 0.15f, 0.85f);
        [SerializeField] private Color _discoveredColor = Color.white;
        [SerializeField] private Color _currentColor = new Color(1f, 0.9f, 0.4f, 1f);

        private readonly List<Image> _chamberSlots = new List<Image>();

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (_floorContainer == null || _chamberSlotPrefab == null) return;

            foreach (Transform child in _floorContainer) Destroy(child.gameObject);
            _chamberSlots.Clear();

            var tracker = ChamberDiscoveryTracker.Instance;
            string currentId = SaveSystem.Current.CurrentChamberId;

            for (int floor = _floorCount; floor >= 1; floor--)
            {
                GameObject row;
                if (_floorRowPrefab != null)
                {
                    row = Instantiate(_floorRowPrefab, _floorContainer);
                }
                else
                {
                    row = new GameObject($"Floor_{floor}");
                    row.transform.SetParent(_floorContainer, false);
                    row.AddComponent<RectTransform>();
                    row.AddComponent<HorizontalLayoutGroup>();
                }

                if (tracker != null)
                {
                    foreach (var chamber in tracker.ChambersOnFloor(floor))
                    {
                        var slot = Instantiate(_chamberSlotPrefab, row.transform);
                        var img = slot.GetComponentInChildren<Image>();
                        if (img != null)
                        {
                            bool discovered = tracker.IsDiscovered(chamber.ChamberId);
                            bool isCurrent = chamber.ChamberId == currentId;
                            img.sprite = discovered ? chamber.DiscoveredSprite : chamber.UndiscoveredSprite;
                            img.color = isCurrent ? _currentColor : (discovered ? _discoveredColor : _undiscoveredColor);
                            _chamberSlots.Add(img);
                        }
                    }
                }
            }

            if (_currentFloorLabel != null)
                _currentFloorLabel.text = $"Floor {SaveSystem.Current.CurrentFloor}";
            if (_discoveryCountLabel != null && tracker != null)
                _discoveryCountLabel.text = $"{tracker.Discovered.Count} / {tracker.AllChambers.Count}";
        }
    }
}
