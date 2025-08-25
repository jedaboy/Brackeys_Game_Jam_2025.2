using GRD.SceneManagement;

namespace BGJ_14
{
    public class ExpeditionTestSceneController : SceneController<ExpeditionTestSceneData>
    {

    }

    public class ExpeditionTestSceneData : ISceneData<ExpeditionTestSceneController>
    {
        public string SceneName => "ExpeditionTestScene";
    }
}
