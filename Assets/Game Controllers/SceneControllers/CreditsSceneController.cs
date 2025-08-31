using GRD.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSceneController : AdditiveSceneController<CreditsSceneData>
{
    [SerializeField] private Button _closeButton;

    public override Task OnLoad()
    {
        _closeButton.onClick.AddListener(CloseCredits);
        return base.OnLoad();
    }

    private async void CloseCredits() 
    {
        await SceneOrchestrator.UnloadAdditiveScene(this);
    }
}

public class CreditsSceneData : IAdditiveSceneData<CreditsSceneController>
{
    public string SceneName => "CreditsScene";
}
