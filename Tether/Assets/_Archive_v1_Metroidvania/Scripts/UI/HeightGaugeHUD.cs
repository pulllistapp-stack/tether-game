using UnityEngine;
using UnityEngine.UI;
using Tether.Map;
using Tether.Save;

namespace Tether.UI
{
    public class HeightGaugeHUD : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Transform _gaugeRoot;
        [SerializeField] private GameObject _floorMarkerPrefab;
        [SerializeField] private RectTransform _currentArrow;
        [SerializeField] private RectTransform _maxRecord;

        [Header("Settings")]
        [SerializeField, Range(1, 10)] private int _floorCount = 7;
        [SerializeField] private float _floorSpacing = 56f;
        [SerializeField] private Color _normalColor = new Color(0.8f, 0.8f, 0.9f, 0.7f);
        [SerializeField] private Color _activeColor = new Color(1f, 0.9f, 0.4f, 1f);

        private RectTransform[] _floorMarkers;

        private void Start()
        {
            BuildGauge();
            UpdateGauge();

            if (ChamberDiscoveryTracker.Instance != null)
                ChamberDiscoveryTracker.Instance.OnChamberEntered += OnChamberEntered;
        }

        private void OnDestroy()
        {
            if (ChamberDiscoveryTracker.Instance != null)
                ChamberDiscoveryTracker.Instance.OnChamberEntered -= OnChamberEntered;
        }

        private void OnChamberEntered(ChamberData data) => UpdateGauge();

        private void BuildGauge()
        {
            if (_gaugeRoot == null || _floorMarkerPrefab == null) return;
            _floorMarkers = new RectTransform[_floorCount];
            for (int i = 0; i < _floorCount; i++)
            {
                var go = Instantiate(_floorMarkerPrefab, _gaugeRoot);
                var rt = go.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(0f, i * _floorSpacing);
                    _floorMarkers[i] = rt;
                }
                var txt = go.GetComponentInChildren<Text>();
                if (txt != null) txt.text = (i + 1).ToString();
            }
        }

        private void UpdateGauge()
        {
            int currentFloor = SaveSystem.Current.CurrentFloor;
            int maxFloor = Mathf.Max(1, Mathf.CeilToInt(SaveSystem.Current.MaxHeightReached));

            if (_floorMarkers != null)
            {
                for (int i = 0; i < _floorMarkers.Length; i++)
                {
                    var rt = _floorMarkers[i];
                    if (rt == null) continue;
                    var img = rt.GetComponent<Image>();
                    if (img != null)
                        img.color = (i + 1) == currentFloor ? _activeColor : _normalColor;
                }
            }

            if (_currentArrow != null)
                _currentArrow.anchoredPosition = new Vector2(_currentArrow.anchoredPosition.x, (currentFloor - 1) * _floorSpacing);
            if (_maxRecord != null)
                _maxRecord.anchoredPosition = new Vector2(_maxRecord.anchoredPosition.x, (maxFloor - 1) * _floorSpacing);
        }
    }
}
