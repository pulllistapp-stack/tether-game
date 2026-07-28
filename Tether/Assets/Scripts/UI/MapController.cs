using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tether.UI
{
    /// <summary>
    /// Phase 7C map scene controller. Renders one clickable button per node
    /// from RunSession.Map in a vertical column (bottom-to-top). Only the
    /// next unlocked node is interactive.
    /// </summary>
    public class MapController : MonoBehaviour
    {
        [SerializeField] private RectTransform _nodesRoot;
        [SerializeField] private Text _headerLabel;
        [SerializeField] private Text _stateLabel;
        [SerializeField] private string _arenaSceneName = "Arena_Test";

        private Font _font;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            Time.timeScale = 1f;

            var session = Meta.RunSession.Instance;
            if (session == null || !session.RunActive || session.Map == null || session.Map.Count == 0)
            {
                // Reached map without a run — kick back to main menu
                SceneManager.LoadScene("MainMenu");
                return;
            }
        }

        private void Start()
        {
            BuildNodes();
        }

        private void BuildNodes()
        {
            if (_nodesRoot == null) return;
            for (int i = _nodesRoot.childCount - 1; i >= 0; i--)
                Destroy(_nodesRoot.GetChild(i).gameObject);

            var session = Meta.RunSession.Instance;
            if (session == null) return;

            if (_headerLabel != null) _headerLabel.text = "SCARLET DEVIL TOWER";
            if (_stateLabel != null)
            {
                var wallet = Meta.CoinWallet.Instance;
                var hp = session.CachedPlayerHp;
                var maxHp = session.CachedPlayerMaxHp;
                var relics = Systems.RelicSystem.Instance;
                int relicCount = relics != null ? relics.Owned.Count : 0;
                int coins = wallet != null ? wallet.RunCoins : 0;
                string hpStr = hp >= 0 ? (hp + "/" + maxHp) : "-";
                _stateLabel.text = "HP  " + hpStr + "     COINS  " + coins + "     RELICS  " + relicCount;
            }

            float rowSpacing = 90f;
            float rowH = 70f;

            for (int i = 0; i < session.Map.Count; i++)
            {
                var node = session.Map[i];
                bool available = session.IsNodeAvailable(i);
                bool cleared = node.cleared;
                BuildNodeButton(node, i, available, cleared, i * (rowSpacing), rowH);
            }
        }

        private void BuildNodeButton(Meta.MapNode node, int index, bool available, bool cleared, float y, float rowH)
        {
            var rowGO = new GameObject("Node_" + index + "_" + node.type, typeof(RectTransform));
            rowGO.transform.SetParent(_nodesRoot, false);
            var rt = rowGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0f); rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, y);
            rt.sizeDelta = new Vector2(420f, rowH);

            var img = rowGO.AddComponent<Image>();
            img.color = NodeColor(node.type, available, cleared);

            var btn = rowGO.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.interactable = available;
            int idx = index;
            btn.onClick.AddListener(() => OnNodeClicked(idx));

            var lblGO = new GameObject("Label", typeof(RectTransform));
            lblGO.transform.SetParent(rowGO.transform, false);
            var lrt = lblGO.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(20f, 0f); lrt.offsetMax = new Vector2(-20f, 0f);
            var lTxt = lblGO.AddComponent<Text>();
            lTxt.font = _font; lTxt.fontSize = 26; lTxt.color = new Color(0.05f, 0.05f, 0.1f);
            lTxt.alignment = TextAnchor.MiddleCenter; lTxt.fontStyle = FontStyle.Bold;
            string status = cleared ? "  ✓" : (available ? "" : "  ✗");
            lTxt.text = NodeName(node.type) + status;
        }

        private void OnNodeClicked(int index)
        {
            var session = Meta.RunSession.Instance;
            if (session == null || !session.IsNodeAvailable(index)) return;
            session.SetCurrentNode(index);

            var node = session.Map[index];
            if (node.type == Meta.NodeType.Rest)
            {
                // Instant heal + mark cleared without loading arena
                if (session.CachedPlayerMaxHp > 0 && session.CachedPlayerHp >= 0)
                {
                    int healed = Mathf.Min(session.CachedPlayerMaxHp, session.CachedPlayerHp + 3);
                    session.CachePlayerHp(healed, session.CachedPlayerMaxHp);
                }
                session.MarkCurrentCleared();
                BuildNodes();
                return;
            }

            if (node.type == Meta.NodeType.Shop)
            {
                // TODO: mini-shop UI. For now heal 1 and clear.
                if (session.CachedPlayerMaxHp > 0 && session.CachedPlayerHp >= 0)
                {
                    int healed = Mathf.Min(session.CachedPlayerMaxHp, session.CachedPlayerHp + 1);
                    session.CachePlayerHp(healed, session.CachedPlayerMaxHp);
                }
                session.MarkCurrentCleared();
                BuildNodes();
                return;
            }

            SceneManager.LoadScene(_arenaSceneName);
        }

        private static string NodeName(Meta.NodeType t)
        {
            switch (t)
            {
                case Meta.NodeType.Combat: return "COMBAT";
                case Meta.NodeType.Elite:  return "★ ELITE";
                case Meta.NodeType.Rest:   return "REST ✿";
                case Meta.NodeType.Shop:   return "SHOP";
                case Meta.NodeType.Boss:   return "☠ BOSS ☠";
                default: return t.ToString();
            }
        }

        private static Color NodeColor(Meta.NodeType t, bool available, bool cleared)
        {
            Color baseC;
            switch (t)
            {
                case Meta.NodeType.Combat: baseC = new Color(0.85f, 0.9f, 1f); break;
                case Meta.NodeType.Elite:  baseC = new Color(1f, 0.85f, 0.3f); break;
                case Meta.NodeType.Rest:   baseC = new Color(0.5f, 0.95f, 0.6f); break;
                case Meta.NodeType.Shop:   baseC = new Color(1f, 0.7f, 0.4f); break;
                case Meta.NodeType.Boss:   baseC = new Color(0.95f, 0.35f, 0.35f); break;
                default:                   baseC = Color.white; break;
            }
            if (cleared) return new Color(baseC.r * 0.35f, baseC.g * 0.35f, baseC.b * 0.35f, 1f);
            if (!available) return new Color(baseC.r * 0.5f, baseC.g * 0.5f, baseC.b * 0.5f, 0.4f);
            return baseC;
        }
    }
}
