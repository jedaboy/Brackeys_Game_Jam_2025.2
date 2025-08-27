using UnityEngine;

namespace BGJ_14
{
    [CreateAssetMenu(fileName = "Lithium Flask Item", menuName = "Upgrade Itens/Lithium Flask")]
    public class LithiumFlaskItem : UpgradeItem
    {
        public override int GetCost(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.GetLithiumFlaskCost();
        }

        public override bool IsAvailable(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.CheckLithiumFlaskAvailability();
        }

        public override void OnBuy(int gearCost, PlayerProgress playerProgress)
        {
            playerProgress.BuyLithiumFlask(gearCost);
        }
    }
}
