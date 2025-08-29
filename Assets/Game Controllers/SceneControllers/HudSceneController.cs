using BGJ14;
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

        [SerializeField] private Transform _batteryBarFill;
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
            _batteryText.text = Mathf.Ceil(sceneData.playerRobot.battery.CurrentCharge).ToString();
            _batteryBarFill.localScale = new Vector3(
                sceneData.playerRobot.battery.NormalizedCurrentCharge,
                1,
                1);
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
        public RobotController playerRobot { get; }

        public HudSceneData(PlayerBag playerBag, RobotController playerRobot) 
        {
            this.playerBag = playerBag;
            this.playerRobot = playerRobot;
        }
    }
}
