using BGJ_14;
using BGJ14;
using GRD.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;

public class ExpeditionSceneController : SceneController<ExpeditionSceneData>
{
    private GameSessionService _gameSessionService;
    private PlayerProgress _playerProgress => _gameSessionService.playerProgress;
    private PlayerBag _playerBag => _playerProgress.playerBag;
    private HudSceneController _hud;
    private bool _gameOverStarted;
    private GameOverSceneController _gameOverMenu;

    [SerializeField] private ExpeditionManager _expeditionManager;
    [SerializeField] private RobotController _playerRobot;

    public ExpeditionManager expeditionManager => _expeditionManager;

    public override Task OnLoad()
    {
        _gameOverStarted = false;

        _gameSessionService =
            GameManager.instance.GetService<GameSessionService>();

        return base.OnLoad();
    }

    public override async Task OnPostLoad()
    {
        _hud = await SceneOrchestrator.LoadSceneAdditive(new HudSceneData(_playerBag, _playerRobot));
        _playerRobot.battery.onBatteryUpdate += _hud.UpdateBattery;
        await base.OnPostLoad();
    }

    private void Update()
    {
        DetectPlayerDied();
    }

    public void StartExpedition()
    {
        _expeditionManager.StartExpedition();
        _hud.UpdateBattery();
        _hud.UpdateAmmo();
        _hud.UpdateGears();
        _hud.UpdateLithiumFlasks();
    }

    public void EndExpedition()
    {
        _expeditionManager.EndExpedition();
    }

    public async void OpenShop()
    {
        _hud.HideHud();

        //TODO: bloquear ações do jogador

        UpgradeMenuSceneController upgradeMenu =
            await SceneOrchestrator.LoadSceneAdditive(new UpgradeMenuSceneData());
        await upgradeMenu.WaitForExit;
        _hud.ShowHud();

        //TODO: reabilitar ações do jogador
    }

    public async void DetectPlayerDied() 
    {
        if (_gameOverStarted)
            return;

        if (_playerRobot.battery.currentCharge <= 0) 
        {
            _gameOverStarted = true;
            _gameOverMenu = await SceneOrchestrator.LoadSceneAdditive(new GameOverSceneData());
        }
    }
}

public class ExpeditionSceneData : ISceneData<ExpeditionSceneController>
{
    public string SceneName => "Mapa";
}
