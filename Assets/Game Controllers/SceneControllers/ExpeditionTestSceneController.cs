using BGJ14;
using GRD.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;

namespace BGJ_14
{
    public class ExpeditionTestSceneController : SceneController<ExpeditionTestSceneData>
    {
        private GameSessionService _gameSessionService;
        private PlayerProgress _playerProgress => _gameSessionService.playerProgress;
        private PlayerBag _playerBag => _playerProgress.playerBag;
        private HudSceneController _hud;

        [SerializeField] private ExpeditionManager _expeditionManager;
        [SerializeField] private RobotController _playerRobot;

        public override Task OnLoad()
        {
            _gameSessionService = 
                GameManager.instance.GetService<GameSessionService>();

            return base.OnLoad();
        }

        public override async Task OnPostLoad()
        {
            _hud = await SceneOrchestrator.LoadSceneAdditive(new HudSceneData(_playerBag, _playerRobot));
            await base.OnPostLoad();
        }

        [EButton]
        private void StartExpedition() 
        {
            _expeditionManager.StartExpedition();
            _hud.UpdateBattery();
            _hud.UpdateAmmo();
            _hud.UpdateGears();
            _hud.UpdateLithiumFlasks();
        }

        [EButton]
        private void DebugGetGear() 
        {
            _playerBag.AddGear(1);
            _hud.UpdateGears();
        }

        [EButton]
        private void EndExpedition() 
        {
            _expeditionManager.EndExpedition();
        }

        [EButton]
        private async void DebugOpenShop() 
        {
            _hud.HideHud();
            UpgradeMenuSceneController upgradeMenu = 
                await SceneOrchestrator.LoadSceneAdditive(new UpgradeMenuSceneData());
            await upgradeMenu.WaitForExit;
            _hud.ShowHud();
        }
    }

    public class ExpeditionTestSceneData : ISceneData<ExpeditionTestSceneController>
    {
        public string SceneName => "ExpeditionTestScene";
    }
}
