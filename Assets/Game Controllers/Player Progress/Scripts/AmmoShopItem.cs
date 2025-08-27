using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGJ_14
{
    [CreateAssetMenu(fileName = "Ammo Item", menuName = "Upgrade Itens/Ammo")]
    public class AmmoShopItem : UpgradeItem
    {
        [SerializeField] private int _ammoAmount;
        [SerializeField] private int _discount;

        public override int GetCost(PlayerProgress.ShopProgress shopProgress)
        {
            return shopProgress.GetAmmoCost(_ammoAmount, _discount);
        }

        public override bool IsAvailable(PlayerProgress.ShopProgress shopProgress)
        {
            return true;
        }

        public override void OnBuy(int gearCost, PlayerProgress playerProgress)
        {
            playerProgress.BuyAmmo(gearCost, _ammoAmount);
        }
    }
}
