using GRD.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BGJ_14
{
    public class HudSceneController : AdditiveSceneController<HudSceneData>
    {
        [SerializeField] private GameObject _hudParent;

        [SerializeField] private Image _batteryBarFill;
        [SerializeField] private TextMeshProUGUI _batteryText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private TextMeshProUGUI _gearsText;
        [SerializeField] private TextMeshProUGUI _lithiumFlasksText;

        public override Task OnLoad()
        {
            UpdateBattery();
            UpdateAmmo();
            UpdateGears();
            UpdateLithiumFlasks();
            return base.OnLoad();
        }

        public void HideHud() 
        {
            _hudParent.SetActive(false);
        }

        public void ShowHud() 
        {
            UpdateBattery();
            UpdateAmmo();
            UpdateGears();
            UpdateLithiumFlasks();
            _hudParent.SetActive(true);
        }

        public void UpdateBattery() 
        {
            //TODO: receber vida do robô
        }

        public void UpdateAmmo() 
        {
            _ammoText.text = $"x{sceneData.playerBag.ammo}";
        }

        public void UpdateGears() 
        {
            _gearsText.text = $"x{sceneData.playerBag.gears}";
        }

        public void UpdateLithiumFlasks() 
        {
            _lithiumFlasksText.text = $"x{sceneData.playerBag.lithiumFlask}";
        }
    }

    public class HudSceneData : IAdditiveSceneData<HudSceneController>
    {
        public string SceneName => "HudScene";

        public PlayerBag playerBag { get; }

        public HudSceneData(PlayerBag playerBag) 
        {
            //TODO: receber o Controller do robô para obter a vida
            this.playerBag = playerBag;
        }
    }
}
