using UnityEngine;

namespace Tether.Meta
{
    /// <summary>
    /// Phase 5 run startup applier. On scene load, reads all ShopItems and
    /// applies effects of the unlocked ones (extra ball slots, +HP, etc).
    /// Lives on a scene GameObject next to CoinWallet / RunStats.
    /// </summary>
    public class RunStartup : MonoBehaviour
    {
        [SerializeField] private ShopItem[] _allItems;

        private void Start()
        {
            var slots = FindFirstObjectByType<Player.BallSlotManager>();
            var hp    = FindFirstObjectByType<Player.PlayerHealth>();
            var nova  = FindFirstObjectByType<Player.NovaBomb>();
            var wallet= FindFirstObjectByType<CoinWallet>();

            if (_allItems == null) return;
            foreach (var item in _allItems)
            {
                if (item == null) continue;
                if (!MetaSave.IsUnlocked(item.id)) continue;

                switch (item.effect)
                {
                    case ShopEffect.UnlockBall:
                        if (slots != null && item.ballDataUnlock != null)
                            slots.AddSlotIfMissing(item.ballDataUnlock);
                        break;
                    case ShopEffect.StartingHpBonus:
                        if (hp != null) hp.ExtendMaxHp(Mathf.RoundToInt(item.magnitude));
                        break;
                    case ShopEffect.StartingCoins:
                        if (wallet != null) wallet.AddCoin(Mathf.RoundToInt(item.magnitude));
                        break;
                    case ShopEffect.FasterNova:
                        if (nova != null) nova.ReduceCooldown(item.magnitude);
                        break;
                    case ShopEffect.StartingRelic:
                        if (item.relicToGrant != null && Systems.RelicSystem.Instance != null)
                            Systems.RelicSystem.Instance.Grant(item.relicToGrant);
                        break;
                }
            }
        }
    }
}
