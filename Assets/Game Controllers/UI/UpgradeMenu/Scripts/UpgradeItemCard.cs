using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace BGJ_14
{
    public class UpgradeItemCard : MonoBehaviour
    {
        private UpgradeItem _myItem;

        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private GameObject _costArea;
        [SerializeField] private TextMeshProUGUI _itemCost;
        [SerializeField] private Button _cardButton;

        [SerializeField] private Color _unavailableItemColor;


        public void SetupCard(UpgradeItem item, Action<UpgradeItem> onClick) 
        {
            _myItem = item;
            _cardButton.onClick.AddListener(() => onClick(_myItem));

            UpdateCardData();
        }

        public void UpdateCardData() 
        {
            PlayerProgress.ShopProgress shopProgress =
                GameManager.instance.GetService<GameSessionService>().playerProgress.shopProgress;
            _itemImage.sprite = _myItem.itemImage;
            _itemName.text = _myItem.itemName;
            _itemCost.text = $"x{_myItem.GetCost(shopProgress)}";

            _costArea.SetActive(_myItem.IsAvailable(shopProgress));
            _itemImage.color = _myItem.IsAvailable(shopProgress)
                ? Color.white
                : _unavailableItemColor;
        }
    }
}
