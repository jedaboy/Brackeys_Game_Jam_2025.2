using UnityEngine;
using static BGJ_14.PlayerProgress;

namespace BGJ_14
{
    public class PlayerProgress
    {
        private ShopProgress _shopProgress;

        private int _totalGears;
        private float _totalBattery;
        private int _gunLevel;
        private int _lithiumFlaskCount;
        private PlayerBag _playerBag;

        public ShopProgress shopProgress => _shopProgress;

        public int totalGears => _totalGears;
        public float totalBattery => _totalBattery;
        public int gunLevel => _gunLevel;
        public int lithiumFlaskCount => _lithiumFlaskCount;

        public PlayerBag playerBag => _playerBag;

        public PlayerProgress()
        {
            _shopProgress = new ShopProgress();

            _totalGears = 0;
            _totalBattery = 100;
            _gunLevel = 1;
            _lithiumFlaskCount = 0;

            _playerBag = new PlayerBag();
        }

        public void OnExpeditionStart() 
        {
            _playerBag.RestoreLithiumFlasks(_lithiumFlaskCount);
        }

        public void OnExpeditionEnd()
        {
            AddTotalGears(_playerBag.RedeemGears());
            _playerBag.RestoreLithiumFlasks(_lithiumFlaskCount);
        }

        public void AddTotalGears(int gearAmount)
        {
            _totalGears += gearAmount;
        }

        public void UpgradeBattery(int gearCost, float batteryAmount)
        {
            if (_totalGears < gearCost)
                return;

            _totalGears -= gearCost;
            _totalBattery += batteryAmount;

            _shopProgress.RegisterBatterySold();
        }

        public void UpgradeGun(int gearCost)
        {
            if (_totalGears < gearCost)
                return;

            _totalGears -= gearCost;
            _gunLevel++;

            _shopProgress.RegisterGunSold();
        }

        public void BuyLithiumFlask(int gearCost)
        {
            if (_totalGears < gearCost)
                return;

            _totalGears -= gearCost;
            _lithiumFlaskCount++;

            _shopProgress.RegisterLithiumFlaskSold();
            _playerBag.RestoreLithiumFlasks(_lithiumFlaskCount);
        }

        public void BuyAmmo(int gearCost, int ammoCount) 
        {
            if (_totalGears < gearCost)
                return;

            _totalGears -= gearCost;

            _playerBag.AddAmmo(ammoCount);
        }

        public class ShopProgress 
        {
            private int _batteryUpgradesSold;
            private int _gunUpgradesSold;
            private int _lithiumFlaskSold;

            private const int _maxBatteryUpgradesToBuy = 3;
            private const int _maxGunUpgradesToBuy = 10;
            private const int _maxLithiumFlaskToBuy = 5;

            internal void RegisterBatterySold() 
            {
                _batteryUpgradesSold++;
            }

            internal void RegisterGunSold()
            {
                _gunUpgradesSold++;
            }

            internal void RegisterLithiumFlaskSold()
            {
                _lithiumFlaskSold++;
            }

            public int GetBatteryCost() 
            {
                return 10 + 5 * _batteryUpgradesSold;
            }

            public int GetGunCost()
            {
                return 15 + 10 * _gunUpgradesSold;
            }

            public int GetLithiumFlaskCost()
            {
                return 50 + 30 * _lithiumFlaskSold;
            }

            public int GetAmmoCost(int ammoAmount, int discount) 
            {
                return Mathf.Max(Mathf.CeilToInt(ammoAmount / 6) - discount, 1);
            }

            public bool CheckBatteryUpgradeAvailability() 
            {
                return _batteryUpgradesSold < _maxBatteryUpgradesToBuy;
            }

            public bool CheckGunUpgradeAvailability()
            {
                return _gunUpgradesSold < _maxGunUpgradesToBuy;
            }

            public bool CheckLithiumFlaskAvailability()
            {
                return _lithiumFlaskSold < _maxLithiumFlaskToBuy;
            }
        }
    }
}
