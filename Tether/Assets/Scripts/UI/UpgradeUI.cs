using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace Tether.UI
{
    /// <summary>
    /// Phase 3 upgrade card modal. Shown between waves; 3 card slots that
    /// display an UpgradeCard's name/description/tint. Click a slot to pick.
    /// UpgradeSelector supplies the offered list and receives the callback.
    /// </summary>
    public class UpgradeUI : MonoBehaviour
    {
        [Header("Modal")]
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Text _headerText;

        [Header("Card Slots (parent transforms)")]
        [SerializeField] private CardSlot[] _slots;

        [Serializable]
        public class CardSlot
        {
            public Button button;
            public Image background;
            public Text titleText;
            public Text descText;
        }

        private Action<Systems.UpgradeCard> _onPick;
        private readonly List<Systems.UpgradeCard> _current = new List<Systems.UpgradeCard>();

        private void Awake()
        {
            Hide();
            if (_slots != null)
            {
                for (int i = 0; i < _slots.Length; i++)
                {
                    int idx = i;
                    if (_slots[i].button != null)
                        _slots[i].button.onClick.AddListener(() => HandleClick(idx));
                }
            }
        }

        public void Show(List<Systems.UpgradeCard> offered, Action<Systems.UpgradeCard> onPick)
        {
            _onPick = onPick;
            _current.Clear();
            _current.AddRange(offered);

            if (_headerText != null) _headerText.text = "CHOOSE AN UPGRADE";

            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];
                bool has = i < offered.Count && offered[i] != null;
                slot.button.gameObject.SetActive(has);
                if (!has) continue;

                var card = offered[i];
                if (slot.titleText != null) slot.titleText.text = card.displayName.ToUpper();
                if (slot.descText  != null) slot.descText.text  = card.description;
                if (slot.background != null) slot.background.color = card.tint;
            }

            if (_group != null)
            {
                _group.alpha = 1f;
                _group.interactable = true;
                _group.blocksRaycasts = true;
            }
        }

        public void Hide()
        {
            if (_group != null)
            {
                _group.alpha = 0f;
                _group.interactable = false;
                _group.blocksRaycasts = false;
            }
            // The clicked card button stays selected in the EventSystem otherwise,
            // and later re-fires via the keyboard "Submit" action (Space/Enter) —
            // e.g. pressing the dash key would silently "click" it again.
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        private void HandleClick(int idx)
        {
            if (idx < 0 || idx >= _current.Count) return;
            var card = _current[idx];
            var cb = _onPick;
            _onPick = null;
            cb?.Invoke(card);
        }
    }
}
