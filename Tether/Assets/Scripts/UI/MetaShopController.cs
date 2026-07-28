using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tether.UI
{
    /// <summary>
    /// Phase 5 shop panel. Toggles on/off from the MainMenu, spends lifetime
    /// coins from MetaSave to unlock ShopItems. Builds row entries in a
    /// vertical layout at Awake — no manual scene wiring per row.
    /// </summary>
    public class MetaShopController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _panelGroup;
        [SerializeField] private Text _coinsLabel;
        [SerializeField] private RectTransform _itemsRoot;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Meta.ShopItem[] _items;

        private readonly List<Row> _rows = new List<Row>();
        private Font _font;

        private class Row
        {
            public Meta.ShopItem item;
            public Text nameText;
            public Text descText;
            public Text costText;
            public Button buyButton;
            public Text buyText;
        }

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            Hide();
            if (_closeButton != null) _closeButton.onClick.AddListener(Hide);
            BuildRows();
        }

        public void Show()
        {
            if (_panelGroup == null) return;
            _panelGroup.alpha = 1f;
            _panelGroup.interactable = true;
            _panelGroup.blocksRaycasts = true;
            RefreshRows();
        }

        public void Hide()
        {
            if (_panelGroup == null) return;
            _panelGroup.alpha = 0f;
            _panelGroup.interactable = false;
            _panelGroup.blocksRaycasts = false;
        }

        private void BuildRows()
        {
            if (_itemsRoot == null || _items == null) return;

            for (int i = _itemsRoot.childCount - 1; i >= 0; i--)
                Destroy(_itemsRoot.GetChild(i).gameObject);
            _rows.Clear();

            float rowH = 90f;
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null) continue;
                _rows.Add(BuildRow(_items[i], i, rowH));
            }
        }

        private Row BuildRow(Meta.ShopItem item, int index, float rowH)
        {
            var rowGO = new GameObject("Row_" + item.id, typeof(RectTransform));
            rowGO.transform.SetParent(_itemsRoot, false);
            var rt = rowGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -index * (rowH + 10f));
            rt.sizeDelta = new Vector2(0f, rowH);
            rt.offsetMin = new Vector2(20f, rt.offsetMin.y);
            rt.offsetMax = new Vector2(-20f, rt.offsetMax.y);

            var bg = rowGO.AddComponent<Image>();
            bg.color = new Color(item.tint.r, item.tint.g, item.tint.b, 0.25f);

            var nameGO = new GameObject("Name", typeof(RectTransform));
            nameGO.transform.SetParent(rowGO.transform, false);
            var nrt = nameGO.GetComponent<RectTransform>();
            nrt.anchorMin = new Vector2(0f, 1f); nrt.anchorMax = new Vector2(0.55f, 1f);
            nrt.pivot = new Vector2(0f, 1f);
            nrt.anchoredPosition = new Vector2(20f, -10f);
            nrt.sizeDelta = new Vector2(0f, 30f);
            var nTxt = nameGO.AddComponent<Text>();
            nTxt.font = _font; nTxt.fontSize = 22; nTxt.color = new Color(1f, 0.95f, 0.85f);
            nTxt.alignment = TextAnchor.MiddleLeft; nTxt.fontStyle = FontStyle.Bold;

            var descGO = new GameObject("Desc", typeof(RectTransform));
            descGO.transform.SetParent(rowGO.transform, false);
            var drt = descGO.GetComponent<RectTransform>();
            drt.anchorMin = new Vector2(0f, 0f); drt.anchorMax = new Vector2(0.55f, 0.55f);
            drt.pivot = new Vector2(0f, 0f);
            drt.anchoredPosition = new Vector2(20f, 8f);
            drt.sizeDelta = new Vector2(0f, 0f);
            drt.offsetMax = new Vector2(-10f, drt.offsetMax.y);
            var dTxt = descGO.AddComponent<Text>();
            dTxt.font = _font; dTxt.fontSize = 15; dTxt.color = new Color(0.75f, 0.8f, 0.9f);
            dTxt.alignment = TextAnchor.UpperLeft;
            dTxt.horizontalOverflow = HorizontalWrapMode.Wrap;

            var costGO = new GameObject("Cost", typeof(RectTransform));
            costGO.transform.SetParent(rowGO.transform, false);
            var crt = costGO.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.55f, 0f); crt.anchorMax = new Vector2(0.75f, 1f);
            crt.offsetMin = Vector2.zero; crt.offsetMax = Vector2.zero;
            var cTxt = costGO.AddComponent<Text>();
            cTxt.font = _font; cTxt.fontSize = 20; cTxt.color = new Color(1f, 0.85f, 0.3f);
            cTxt.alignment = TextAnchor.MiddleCenter;

            var buyGO = new GameObject("BuyBtn", typeof(RectTransform));
            buyGO.transform.SetParent(rowGO.transform, false);
            var brt = buyGO.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.78f, 0.2f); brt.anchorMax = new Vector2(0.97f, 0.8f);
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;
            var buyImg = buyGO.AddComponent<Image>();
            buyImg.color = new Color(0.85f, 0.9f, 1f, 0.95f);
            var buyBtn = buyGO.AddComponent<Button>();
            buyBtn.targetGraphic = buyImg;

            var buyLblGO = new GameObject("Label", typeof(RectTransform));
            buyLblGO.transform.SetParent(buyGO.transform, false);
            var blrt = buyLblGO.GetComponent<RectTransform>();
            blrt.anchorMin = Vector2.zero; blrt.anchorMax = Vector2.one;
            blrt.offsetMin = Vector2.zero; blrt.offsetMax = Vector2.zero;
            var blTxt = buyLblGO.AddComponent<Text>();
            blTxt.font = _font; blTxt.fontSize = 20; blTxt.color = new Color(0.05f, 0.05f, 0.1f);
            blTxt.alignment = TextAnchor.MiddleCenter; blTxt.fontStyle = FontStyle.Bold;

            var row = new Row { item = item, nameText = nTxt, descText = dTxt,
                                 costText = cTxt, buyButton = buyBtn, buyText = blTxt };
            buyBtn.onClick.AddListener(() => TryBuy(row));
            RefreshRow(row);
            return row;
        }

        private void RefreshRows()
        {
            if (_coinsLabel != null)
                _coinsLabel.text = "COINS  " + Meta.MetaSave.LifetimeCoins;
            foreach (var r in _rows) RefreshRow(r);
        }

        private void RefreshRow(Row r)
        {
            bool owned = Meta.MetaSave.IsUnlocked(r.item.id);
            bool canAfford = Meta.MetaSave.LifetimeCoins >= r.item.cost;

            r.nameText.text = r.item.displayName;
            r.descText.text = r.item.description;
            r.costText.text = owned ? "OWNED" : r.item.cost + "c";
            r.costText.color = owned ? new Color(0.5f, 0.95f, 0.6f)
                                     : (canAfford ? new Color(1f, 0.85f, 0.3f) : new Color(0.8f, 0.5f, 0.5f));

            r.buyText.text = owned ? "✓" : "BUY";
            r.buyButton.interactable = !owned && canAfford;
        }

        private void TryBuy(Row r)
        {
            if (Meta.MetaSave.IsUnlocked(r.item.id)) return;
            if (!Meta.MetaSave.TrySpend(r.item.cost)) return;
            Meta.MetaSave.SetUnlocked(r.item.id, true);
            RefreshRows();
        }
    }
}
