using UnityEngine;

namespace BGJ_14
{
    public abstract class UpgradeItem : ScriptableObject
    {
        [SerializeField] private string _itemName;
        [SerializeField] private Sprite _itemImage;
        [SerializeField] private string _itemDescription;

        public string itemName => _itemName;
        public Sprite itemImage => _itemImage;
        public string itemDescription => _itemDescription;

        public abstract bool IsAvailable(PlayerProgress.ShopProgress shopProgress);

        public abstract int GetCost(PlayerProgress.ShopProgress shopProgress);

        public abstract void OnBuy(int gearCost, PlayerProgress playerProgress);
    }
}
