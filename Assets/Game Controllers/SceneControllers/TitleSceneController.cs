using GRD.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BGJ_14
{
    public class TitleSceneController : SceneController<TitleSceneData>
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _exitGameButton;

        public override Task OnLoad()
        {
            _startGameButton.onClick.AddListener(OnClickStartGame);
            _creditsButton.onClick.AddListener(OnClickCredits);
            _exitGameButton.onClick.AddListener(OnClickExitGame);
            return base.OnLoad();
        }

        private void OnClickStartGame()
        {
            GameSessionService gameSessionService =
                GameManager.instance.GetService<GameSessionService>();

            gameSessionService.StartNewGameSession();

            SceneOrchestrator.LoadScene(new ExpeditionSceneData());
        }

        private void OnClickCredits()
        {

        }

        private void OnClickExitGame()
        {
            Application.Quit();
        }
    }

    public class TitleSceneData : ISceneData<TitleSceneController>
    {
        public string SceneName => "TitleScreen";
    }
}
