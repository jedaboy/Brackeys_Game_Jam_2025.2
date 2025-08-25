using System.Threading.Tasks;
using UnityEngine;

namespace GRD.SceneManagement
{
    public class SceneController<T> : SceneController where T : ISceneData
    {
        new protected T sceneData => (T)base.sceneData;
    }

    public class SceneController : MonoBehaviour
    {
        protected ISceneData sceneData;

        protected bool _initialized = false;

        public virtual void Setup(ISceneData data)
        {
            sceneData = data;
        }

        public virtual Task OnLoad()
        {
            _initialized = true;
            return Task.CompletedTask;
        }

        public virtual Task OnPostLoad()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUnload()
        {
            return Task.CompletedTask;
        }
    }

    public interface ISceneData<T> : ISceneData
    {
        string SceneName { get; }
    }

    public interface ISceneData { }
}
