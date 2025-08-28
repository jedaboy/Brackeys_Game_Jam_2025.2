using GRD.SceneManagement;
using UnityEngine;

namespace BGJ_14
{
    public class LaunchController : MonoBehaviour
    {
        private void Awake()
        {
            //Reset Fake Perspective Angle
            Shader.SetGlobalFloat("_FP_Angle", 0);

            GameObject gameManagerGO = new GameObject("GameManager");
            GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
            gameManager.Initialize();

            LoadTitle();
        }

        private void LoadTitle()
        {
            SceneOrchestrator.LoadScene(new TitleSceneData());
        }
    }
}
