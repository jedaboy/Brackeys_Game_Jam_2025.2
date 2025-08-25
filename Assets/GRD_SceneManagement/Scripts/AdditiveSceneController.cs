using System.Threading.Tasks;
using UnityEngine;

namespace GRD.SceneManagement
{
    public class AdditiveSceneController<TData, TResult> : AdditiveSceneController<TData>
    where TData : IAdditiveSceneData
    {
        protected TResult _sceneResult;

        new protected TaskCompletionSource<TResult> _waitForExitTcs;
        new public Task<TResult> WaitForExit => _waitForExitTcs.Task;

        public override Task OnLoad()
        {
            _waitForExitTcs = new TaskCompletionSource<TResult>();
            _initialized = true;
            return Task.CompletedTask;
        }

        public override Task OnUnload()
        {
            _waitForExitTcs.SetResult(_sceneResult);
            return Task.CompletedTask;
        }
    }

    public class AdditiveSceneController<T> : AdditiveSceneController where T : IAdditiveSceneData
    {
        new protected T sceneData => (T)base.sceneData;
    }

    public class AdditiveSceneController : MonoBehaviour
    {
        protected IAdditiveSceneData sceneData;

        protected bool _initialized = false;

        protected TaskCompletionSource<bool> _waitForExitTcs;

        public Task<bool> WaitForExit => _waitForExitTcs.Task;

        public virtual void Setup(IAdditiveSceneData data)
        {
            sceneData = data;
        }

        public virtual Task OnLoad()
        {
            _waitForExitTcs = new TaskCompletionSource<bool>();
            _initialized = true;
            return Task.CompletedTask;
        }

        public virtual Task OnPostLoad()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUnload()
        {
            _waitForExitTcs.SetResult(true);
            return Task.CompletedTask;
        }
    }

    public interface IAdditiveSceneData<T> : IAdditiveSceneData
    {
        string SceneName { get; }
    }

    public interface IAdditiveSceneData { }
}

