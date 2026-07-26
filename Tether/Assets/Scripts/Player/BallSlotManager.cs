using UnityEngine;
using System;

namespace Tether.Player
{
    /// <summary>
    /// Phase 2 ball slot manager. Holds an ordered list of BallData;
    /// number keys 1..N cycle the active slot. Player queries CurrentData
    /// at fire time to configure the spawned ball.
    /// </summary>
    public class BallSlotManager : MonoBehaviour
    {
        [SerializeField] private Gameplay.BallData[] _slots = new Gameplay.BallData[0];
        [SerializeField] private int _startIndex = 0;

        public int CurrentIndex { get; private set; }
        public int SlotCount => _slots != null ? _slots.Length : 0;
        public Gameplay.BallData CurrentData =>
            (_slots != null && _slots.Length > 0 && CurrentIndex >= 0 && CurrentIndex < _slots.Length)
                ? _slots[CurrentIndex]
                : null;

        public event Action<int, Gameplay.BallData> OnSlotChanged;

        private void Awake()
        {
            CurrentIndex = Mathf.Clamp(_startIndex, 0, Mathf.Max(0, SlotCount - 1));
        }

        private void Start()
        {
            OnSlotChanged?.Invoke(CurrentIndex, CurrentData);
        }

        private void Update()
        {
            if (SlotCount == 0) return;

            // 1..9 keys map to slot 0..8
            for (int i = 0; i < Mathf.Min(9, SlotCount); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    Select(i);
                    return;
                }
            }

            // Q / E cycle
            if (Input.GetKeyDown(KeyCode.Q)) Select((CurrentIndex - 1 + SlotCount) % SlotCount);
            if (Input.GetKeyDown(KeyCode.E)) Select((CurrentIndex + 1) % SlotCount);
        }

        public void Select(int index)
        {
            if (index < 0 || index >= SlotCount) return;
            if (index == CurrentIndex) return;
            CurrentIndex = index;
            OnSlotChanged?.Invoke(CurrentIndex, CurrentData);
        }

        /// <summary>Add a BallData to the roster if it isn't already present.</summary>
        public void AddSlotIfMissing(Gameplay.BallData data)
        {
            if (data == null) return;
            if (_slots != null)
            {
                foreach (var s in _slots)
                    if (s == data) return;
            }

            int oldLen = _slots != null ? _slots.Length : 0;
            var newArr = new Gameplay.BallData[oldLen + 1];
            for (int i = 0; i < oldLen; i++) newArr[i] = _slots[i];
            newArr[oldLen] = data;
            _slots = newArr;
        }
    }
}
