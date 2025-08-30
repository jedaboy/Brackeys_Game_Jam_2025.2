using BGJ_14;
using GRD.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameOverSceneController : AdditiveSceneController<GameOverSceneData>
{
    [SerializeField] private Image _bgImage;
    private TaskCompletionSource<bool> _bgAnimationTcs;
    [SerializeField] private float _bgAnimationSpeed = 1;

    [SerializeField] private CanvasGroup _gameOverCanvasGroup;
    private TaskCompletionSource<bool> _gameOverAnimationTcs;
    [SerializeField] private float _gameOverAnimationSpeed = 1;

    [SerializeField] private GameObject _buttonsContainer;
    [SerializeField] private Button _tryAgainButton;
    [SerializeField] private Button _mainMenuButton;

    public override async Task OnLoad()
    {
        _buttonsContainer.SetActive(false);
        Color c = _bgImage.color;
        c.a = 0;
        _bgImage.color = c;
        _gameOverCanvasGroup.alpha = 0;

        _bgAnimationTcs = new TaskCompletionSource<bool>();
        await _bgAnimationTcs.Task;

        _gameOverAnimationTcs = new TaskCompletionSource<bool>();
        await _gameOverAnimationTcs.Task;

        _buttonsContainer.SetActive(true);
        _tryAgainButton.onClick.AddListener(OnClickTryAgain);
        _mainMenuButton.onClick.AddListener(OnClickMainMenu);

        await base.OnLoad();
    }

    private void Update()
    {
        if (_bgAnimationTcs != null && !_bgAnimationTcs.Task.IsCompleted) 
        {
            Color c = _bgImage.color;
            c.a += _bgAnimationSpeed * Time.deltaTime;
            _bgImage.color = c;
            if (c.a >= 1) 
            {
                _bgAnimationTcs.SetResult(true);
            }
        }

        if (_gameOverAnimationTcs != null && !_gameOverAnimationTcs.Task.IsCompleted)
        {
            _gameOverCanvasGroup.alpha += _gameOverAnimationSpeed * Time.deltaTime;
            if (_gameOverCanvasGroup.alpha >= 1)
            {
                _gameOverAnimationTcs.SetResult(true);
            }
        }
    }

    private void OnClickTryAgain() 
    {
        GameSessionService gameSessionService =
                GameManager.instance.GetService<GameSessionService>();

        gameSessionService.StartNewGameSession();

        SceneOrchestrator.LoadScene(new ExpeditionSceneData());
    }

    private void OnClickMainMenu() 
    {
        SceneOrchestrator.LoadScene(new TitleSceneData());
    }
}

public class GameOverSceneData : IAdditiveSceneData<GameOverSceneController>
{
    public string SceneName => "GameOverScene";
}
