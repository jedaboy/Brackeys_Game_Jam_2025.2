using BGJ_14;
using BGJ14;
using GRD.SceneManagement;
using System;
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
    [SerializeField] private Battery _bossBattery;

    public ExpeditionManager expeditionManager => _expeditionManager;

    public override Task OnLoad()
    {
        _gameOverStarted = false;

        _gameSessionService =
            GameManager.instance.GetService<GameSessionService>();
        _playerProgress.onUpgradeBattery += OnPlayerUpgradeBattery;
        _playerProgress.onUpgradeGun += OnPlayerUpgradeGun;

        GameManager.instance.GetService<CursorService>().AddCursorUser(this);

        return base.OnLoad();
    }

    public override async Task OnPostLoad()
    {
        _hud = await SceneOrchestrator.LoadSceneAdditive(new HudSceneData(_playerBag, _playerRobot));
        _playerRobot.battery.onBatteryUpdate += _hud.UpdateBattery;
        _playerRobot.OnAmmoUpdate += OnPlayerAmmoUpdate;
        _playerRobot.OnLBUpdate += OnPlayerLBUpdate;
        _playerRobot.OnCollectGear += OnPlayerCollectGear;
        _playerRobot.OnGetGears += OnPlayerGetGears;
        _playerRobot.OnDropGears += OnPlayerDropGear;
        await base.OnPostLoad();
    }

    public override Task OnUnload()
    {
        _playerRobot.battery.onBatteryUpdate -= _hud.UpdateBattery;
        _playerRobot.OnAmmoUpdate -= OnPlayerAmmoUpdate;
        _playerRobot.OnLBUpdate -= OnPlayerLBUpdate;
        _playerRobot.OnCollectGear -= OnPlayerCollectGear;
        _playerRobot.OnGetGears -= OnPlayerGetGears;
        _playerRobot.OnDropGears -= OnPlayerDropGear;
        return base.OnUnload();
    }

    private void Update()
    {
        DetectPlayerDied();
        DetectBossDied();
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

        _playerRobot.IsStoreOpen = true;

        UpgradeMenuSceneController upgradeMenu =
            await SceneOrchestrator.LoadSceneAdditive(new UpgradeMenuSceneData());
        await upgradeMenu.WaitForExit;
        _hud.ShowHud();

        _playerRobot.IsStoreOpen = false;
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

    private async void DetectBossDied() 
    {
        if (_gameOverStarted)
            return;

        if (_bossBattery.currentCharge <= 0) 
        {
            _gameOverStarted = true;
            SceneOrchestrator.LoadScene(new YouWinSceneData());
        }
    }

    private void OnPlayerCollectGear(int gearAmount)
    {
        _playerBag.AddGear(gearAmount);
        _playerBag.AddGear(gearAmount);
        _hud.UpdateGears();
    }

    private void OnPlayerDropGear(int gearAmount)
    {
        _playerBag.DropGears(gearAmount);
        _hud.UpdateGears();
    }

    private bool OnPlayerAmmoUpdate(int ammoAmount)
    {
        bool canShoot = _playerBag.UseAmmo(ammoAmount);
        _hud.UpdateAmmo();
        return canShoot;
    }
    private bool OnPlayerLBUpdate()
    {
        bool canHeal = _playerBag.UseLithiumFlask();
        _hud.UpdateLithiumFlasks();
        return canHeal;
    }
    
    private int OnPlayerGetGears()
    {
        int numberOfGears = _playerBag.GetGears();
        
        return numberOfGears;
    }
	  private void OnPlayerUpgradeBattery(float maxBatteryCharge) 
    {
        _playerRobot.battery.maxCharge = maxBatteryCharge;
        _playerRobot.battery.currentCharge = maxBatteryCharge;
    }

    private void OnPlayerUpgradeGun(int gunLevel) 
    {
        _playerRobot.Setup(gunLevel);
    }
}

public class ExpeditionSceneData : ISceneData<ExpeditionSceneController>
{
    public string SceneName => "Mapa";
}
