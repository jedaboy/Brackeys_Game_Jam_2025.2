using GRD.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BGJ_14
{
    public class UpgradeMenuSceneController : AdditiveSceneController<UpgradeMenuSceneData>
    {
        private GameSessionService _gameSessionService;
        private PlayerProgress _playerProgress => _gameSessionService.playerProgress;
        private PlayerBag _playerBag => _playerProgress.playerBag;

        [SerializeField] private UpgradeItem[] _shopItens;
        private UpgradeItem _selectedItem;

        [Header("Gears Display")]
        [SerializeField] private TextMeshProUGUI _gearsAmountText;

        [Header("Item List")]
        [SerializeField] private UpgradeItemCard _upgradeItemCardPrefab;
        [SerializeField] private Transform _upgradeItemListParent;
        private List<UpgradeItemCard> _instantiatedItemCards = new List<UpgradeItemCard>();

        [Header("Description Box")]
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonText;

        [Header("Player Data")]
        [SerializeField] private TextMeshProUGUI _playerBattery;
        [SerializeField] private TextMeshProUGUI _playerGunLevel;
        [SerializeField] private TextMeshProUGUI _playerLithiumFlask;
        [SerializeField] private TextMeshProUGUI _playerAmmo;

        [Header("Back Button")]
        [SerializeField] private Button _backButton;

        public override Task OnLoad()
        {
            _gameSessionService = GameManager.instance.GetService<GameSessionService>();

            SetupItemList();
            UpdateGearsDisplay();
            UpdatePlayerData();
            UpdateDescriptionBox(null);

            _buyButton.onClick.AddListener(OnClickBuy);
            _backButton.onClick.AddListener(CloseShop);

            return base.OnLoad();
        }

        private void SetupItemList() 
        {
            foreach (UpgradeItem item in _shopItens) 
            {
                UpgradeItemCard upgradeItemCard = 
                    Instantiate(_upgradeItemCardPrefab, _upgradeItemListParent);
                upgradeItemCard.SetupCard(item, SelectItem);

                _instantiatedItemCards.Add(upgradeItemCard);
            }
        }

        private void SelectItem(UpgradeItem item) 
        {
            _selectedItem = item;
            UpdateDescriptionBox(item);
        }

        private void UpdateDescriptionBox(UpgradeItem? selectedItem) 
        {
            if (selectedItem == null) 
            {
                _descriptionText.text = "";
                _buyButton.gameObject.SetActive(false);
                return;
            }

            PlayerProgress playerProgress = _gameSessionService.playerProgress;
            PlayerProgress.ShopProgress shopProgress = playerProgress.shopProgress;
            int cost = selectedItem.GetCost(shopProgress);

            _descriptionText.text = selectedItem.itemDescription;
            _buyButton.gameObject.SetActive(selectedItem.IsAvailable(shopProgress));
            _buyButtonText.text = $"x{cost}";

            _buyButton.interactable = cost <= playerProgress.totalGears
                && selectedItem.IsAvailable(shopProgress);
        }

        private void UpdateGearsDisplay() 
        {
            PlayerProgress playerProgress = _gameSessionService.playerProgress;
            _gearsAmountText.text = $"x{playerProgress.totalGears}";
        }

        private void UpdatePlayerData() 
        {
            _playerBattery.text = _playerProgress.totalBattery.ToString();
            _playerGunLevel.text = _playerProgress.gunLevel.ToString();
            _playerLithiumFlask.text = $"x{_playerProgress.lithiumFlaskCount}";
            _playerAmmo.text = $"x{_playerBag.ammo}";
        }

        private void OnClickBuy() 
        {
            if (_selectedItem == null)
                return;

            BuyItem(_selectedItem);
        }

        private void BuyItem(UpgradeItem item) 
        {
            PlayerProgress playerProgress = _gameSessionService.playerProgress;
            PlayerProgress.ShopProgress shopProgress = playerProgress.shopProgress;
            item.OnBuy(item.GetCost(shopProgress), playerProgress);

            foreach (var itemCard in _instantiatedItemCards) 
            {
                itemCard.UpdateCardData();
            }

            UpdateGearsDisplay();
            UpdatePlayerData();
            UpdateDescriptionBox(_selectedItem);
        }

        private async void CloseShop() 
        {
            await SceneOrchestrator.UnloadAdditiveScene(this);
        }
    }

    public class UpgradeMenuSceneData : IAdditiveSceneData<UpgradeMenuSceneController>
    {
        public string SceneName => "UpgradeMenu";
    }
}
