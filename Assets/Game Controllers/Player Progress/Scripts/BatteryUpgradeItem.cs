using UnityEngine;

namespace BGJ_14
{
    [CreateAssetMenu(fileName = "Battery Upgrade Item", menuName = "Upgrade Itens/Battery")]
    public class BatteryUpgradeItem : UpgradeItem
    {
        [SerializeField] private float _batteryAmount;

        public override int GetCost(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.GetBatteryCost();
        }

        public override bool IsAvailable(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.CheckBatteryUpgradeAvailability();
        }

        public override void OnBuy(int gearCost, PlayerProgress playerProgress)
        {
            playerProgress.UpgradeBattery(gearCost, _batteryAmount);
        }
    }
}
