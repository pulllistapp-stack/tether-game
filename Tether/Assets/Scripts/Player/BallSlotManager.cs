using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tether.Player
{
    /// <summary>
    /// Ammo-style ball loadout. The player always holds exactly _maxSlots ball
    /// "charges" (starting all as _startingBall); firing reserves the next
    /// available (not currently in-flight) charge round-robin. A charge only
    /// becomes available again once its ball is destroyed or returns home —
    /// there is no infinite fire, just a fixed 5-ball hand in circulation.
    ///
    /// Level-up "Ball: X" cards call ConvertSlot() to retype one charge (a
    /// starting-ball slot first, so earned specializations aren't overwritten)
    /// rather than adding a new slot — the hand size never grows past _maxSlots.
    /// </summary>
    public class BallSlotManager : MonoBehaviour
    {
        [SerializeField] private Gameplay.BallData _startingBall;
        [SerializeField] private int _maxSlots = 5;

        private class Charge
        {
            public Gameplay.BallData data;
            public bool inFlight;
        }

        private readonly List<Charge> _charges = new List<Charge>();
        private int _cursor;

        public int SlotCount => _charges.Count;
        public int AvailableCount
        {
            get
            {
                int n = 0;
                foreach (var c in _charges) if (!c.inFlight) n++;
                return n;
            }
        }

        /// <summary>Fires when a slot is retyped so HUD/UI can refresh the loadout display.</summary>
        public event Action OnLoadoutChanged;

        private void Awake()
        {
            _charges.Clear();
            for (int i = 0; i < Mathf.Max(1, _maxSlots); i++)
                _charges.Add(new Charge { data = _startingBall, inFlight = false });
        }

        /// <summary>Reserves and returns the next available charge index in round-robin
        /// order, or -1 if every charge is currently in flight.</summary>
        public int ReserveNextAvailable()
        {
            for (int i = 0; i < _charges.Count; i++)
            {
                int idx = (_cursor + i) % _charges.Count;
                if (!_charges[idx].inFlight)
                {
                    _charges[idx].inFlight = true;
                    _cursor = (idx + 1) % _charges.Count;
                    return idx;
                }
            }
            return -1;
        }

        public Gameplay.BallData DataAt(int index) =>
            (index >= 0 && index < _charges.Count) ? _charges[index].data : null;

        /// <summary>Frees a charge so it can be fired again. Called by the Ball itself
        /// when it's destroyed (max bounces, void fall, or return-to-player pickup).</summary>
        public void Release(int index)
        {
            if (index < 0 || index >= _charges.Count) return;
            _charges[index].inFlight = false;
        }

        public bool AllChargesAreType(Gameplay.BallData data)
        {
            if (data == null) return false;
            foreach (var c in _charges)
                if (c.data != data) return false;
            return true;
        }

        /// <summary>Retypes one charge to `data`. Prefers a slot still on the starting
        /// ball so earned specializations elsewhere in the hand aren't overwritten;
        /// falls back to any non-matching slot once no starting-ball slots remain.</summary>
        public void ConvertSlot(Gameplay.BallData data)
        {
            if (data == null || AllChargesAreType(data)) return;

            int targetIndex = -1;
            for (int i = 0; i < _charges.Count; i++)
            {
                if (_charges[i].data == _startingBall && _charges[i].data != data)
                { targetIndex = i; break; }
            }
            if (targetIndex < 0)
            {
                for (int i = 0; i < _charges.Count; i++)
                    if (_charges[i].data != data) { targetIndex = i; break; }
            }
            if (targetIndex < 0) return;

            _charges[targetIndex].data = data;
            OnLoadoutChanged?.Invoke();
        }

        /// <summary>Compact "NORMAL x4 SPLIT x1" style summary of the current hand, grouped
        /// by ball type in slot order. Used by the HUD in place of a single selected type.</summary>
        public string LoadoutSummary()
        {
            var order = new List<Gameplay.BallData>();
            var counts = new Dictionary<Gameplay.BallData, int>();
            foreach (var c in _charges)
            {
                if (c.data == null) continue;
                if (!counts.ContainsKey(c.data)) { counts[c.data] = 0; order.Add(c.data); }
                counts[c.data]++;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < order.Count; i++)
            {
                if (i > 0) sb.Append("  ");
                sb.Append(order[i].displayName.ToUpper()).Append(" x").Append(counts[order[i]]);
            }
            return sb.ToString();
        }
    }
}
