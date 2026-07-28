using UnityEngine;

namespace Tether.Map
{
    [CreateAssetMenu(fileName = "Chamber_", menuName = "Tether/Map/Chamber Data", order = 0)]
    public class ChamberData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _chamberId;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 4)] private string _description;

        [Header("Position in Tower")]
        [SerializeField, Range(1, 7)] private int _floor = 1;
        [SerializeField] private Vector2Int _mapPosition;

        [Header("Visuals")]
        [SerializeField] private Sprite _discoveredSprite;
        [SerializeField] private Sprite _undiscoveredSprite;
        [SerializeField] private ChamberType _type = ChamberType.Normal;

        [Header("Scene")]
        [SerializeField] private string _sceneName;

        public string ChamberId => _chamberId;
        public string DisplayName => _displayName;
        public string Description => _description;
        public int Floor => _floor;
        public Vector2Int MapPosition => _mapPosition;
        public Sprite DiscoveredSprite => _discoveredSprite;
        public Sprite UndiscoveredSprite => _undiscoveredSprite;
        public ChamberType Type => _type;
        public string SceneName => _sceneName;
    }
}
