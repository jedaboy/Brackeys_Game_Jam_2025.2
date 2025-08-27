using UnityEngine;

namespace BGJ_14
{
    public class PlayerBag
    {
        private int _gears;
        private int _ammo;
        private int _lithiumFlask;

        public int gears => _gears;
        public int ammo => _ammo;
        public int lithiumFlask => _lithiumFlask;

        public PlayerBag() 
        {
            _gears = 0;
            _ammo = 30;
            _lithiumFlask = 0;
        }

        public void AddGear(int gearAmount) 
        {
            _gears += gearAmount;
        }

        public int DropGear(int amountToDrop)
        {
            int gearsToDrop = Mathf.Min(amountToDrop, _gears);
            _gears -= gearsToDrop;
            return gearsToDrop;
        }

        public int RedeemGears() 
        {
            int gearsToRedeem = _gears;
            _gears = 0;
            return gearsToRedeem;
        }

        public void AddAmmo(int ammoAmount) 
        {
            _ammo += ammoAmount;
        }

        public bool UseAmmo(int ammoCount) 
        {
            if (_ammo < ammoCount)
                return false;
            _ammo -= ammoCount;
            return true;
        }

        public bool UseLithiumFlask()
        {
            if (_lithiumFlask <= 0)
                return false;
            _lithiumFlask--;
            return true;
        }

        public void RestoreLithiumFlasks(int lithiumFlaskCount) 
        {
            _lithiumFlask = lithiumFlaskCount;
        }
    }
}
