using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GRD.SceneManagement
{
    public class SceneOrchestrator
    {
        private const string LoadingSceneName = "LoadingScene";

        private static bool _onSceneTransition = false;
        public static bool onSceneTransition => _onSceneTransition;

        private static SceneController _activeSceneController = null;
        private static List<AdditiveSceneController> _loadedAdditiveScenes = new List<AdditiveSceneController>();

        private static TaskCompletionSource<bool> _additiveSceneUnloadTcs;

        public static async void LoadScene<T>(ISceneData<T> sceneData) where T : SceneController
        {
            if (_onSceneTransition)
            {
                Debug.LogError("On scene transition. Cannot load scene.");
                return;
            }

            _onSceneTransition = true;

            if (_additiveSceneUnloadTcs != null) 
            {
                await _additiveSceneUnloadTcs.Task;
            }

            if (_activeSceneController != null)
            {
                await _activeSceneController.OnUnload();
            }

            List<AdditiveSceneController> additiveScenesToUnload = _loadedAdditiveScenes.ToList<AdditiveSceneController>();
            foreach (AdditiveSceneController additiveScene in additiveScenesToUnload) 
            {
                await UnloadAdditiveScene(additiveScene);
            }

            var loadingTcs = new TaskCompletionSource<bool>();

            AsyncOperation loadingSceneOperation = SceneManager.LoadSceneAsync(LoadingSceneName);

            loadingSceneOperation.completed += _ => { loadingTcs.SetResult(true); };
            await loadingTcs.Task;

            _loadedAdditiveScenes.Clear();

            var sceneTcs = new TaskCompletionSource<bool>();

            AsyncOperation targetSceneOperation = SceneManager.LoadSceneAsync(sceneData.SceneName);

            targetSceneOperation.completed += _ => { sceneTcs.SetResult(true); };
            await sceneTcs.Task;

            T sceneController = Object.FindObjectOfType<T>();

            if (sceneController == null)
            {
                Debug.LogError("No Scene Controller in the scene");
            }

            _activeSceneController = sceneController;
            sceneController.Setup(sceneData);
            await sceneController.OnLoad();
            _onSceneTransition = false;
            await sceneController.OnPostLoad();
        }

        public static async Task<T> LoadSceneAdditive<T>(IAdditiveSceneData<T> sceneData) where T : AdditiveSceneController
        {
            if (_onSceneTransition)
            {
                Debug.LogError("On scene transition. Cannot load scene additive.");
                return null;
            }

            _onSceneTransition = true;

            var tcs = new TaskCompletionSource<bool>();

            AsyncOperation targetSceneOperation =
                SceneManager.LoadSceneAsync(sceneData.SceneName, LoadSceneMode.Additive);

            targetSceneOperation.completed += _ => { tcs.SetResult(true); };
            await tcs.Task;


            Scene scene = SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1);
            T sceneController = GetAdditiveSceneController<T>(scene);
            _loadedAdditiveScenes.Add(sceneController);

            if (sceneController == null)
            {
                Debug.LogError("No Scene Controller in the scene");
            }

            sceneController.Setup(sceneData);
            await sceneController.OnLoad();
            _onSceneTransition = false;
            await sceneController.OnPostLoad();

            return sceneController;
        }

        public static async Task UnloadAdditiveScene(AdditiveSceneController sceneController)
        {
            if (_additiveSceneUnloadTcs != null) 
            {
                await _additiveSceneUnloadTcs.Task;
            }

            _additiveSceneUnloadTcs = new TaskCompletionSource<bool>();

            Scene scene = SceneManager.GetSceneAt(_loadedAdditiveScenes.IndexOf(sceneController) + 1);

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(scene);

            unloadOperation.completed += _ => {
                _loadedAdditiveScenes.Remove(sceneController);
                _additiveSceneUnloadTcs.SetResult(true); 
            };
            await sceneController.OnUnload();
            await _additiveSceneUnloadTcs.Task;
        }

        private static T GetAdditiveSceneController<T>(Scene scene) where T : AdditiveSceneController
        {
            T sceneController = null;
            GameObject[] sceneRootObjects = scene.GetRootGameObjects();
            foreach (GameObject go in sceneRootObjects)
            {
                T controller = go.GetComponent<T>();
                if (controller != null)
                {
                    sceneController = controller;
                    break;
                }
            }

            return sceneController;
        }
    }
}
