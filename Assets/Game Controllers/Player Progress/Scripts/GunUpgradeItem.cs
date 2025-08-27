using UnityEngine;

namespace BGJ_14
{
    [CreateAssetMenu(fileName = "Gun Upgrade Item", menuName = "Upgrade Itens/Gun Upgrade")]
    public class GunUpgradeItem : UpgradeItem
    {
        public override int GetCost(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.GetGunCost();
        }

        public override bool IsAvailable(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.CheckGunUpgradeAvailability();
        }

        public override void OnBuy(int gearCost, PlayerProgress playerProgress)
        {
            playerProgress.UpgradeGun(gearCost);
        }
    }
}
