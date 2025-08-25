using UnityEngine;
using UnityEngine.EventSystems;

namespace BGJ_14
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager instance => _instance;

        public void Initialize()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            CreateManagers();
        }

        #region Managers
        private void CreateManagers()
        {
            CreateEventSystem();

            CreateManagerGameObject<ObjectPoolManager>("[ObjectPoolManager]");
        }

        private T CreateManagerGameObject<T>(string objectName, bool childOfGameManager = true) where T : MonoBehaviour
        {
            GameObject managerGO = new GameObject(objectName);
            if (childOfGameManager)
                managerGO.transform.SetParent(transform);
            T manager = managerGO.AddComponent<T>();
            return manager;
        }

        private void CreateEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem",
                typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.GetComponent<EventSystem>().sendNavigationEvents = false;
            DontDestroyOnLoad(eventSystem);
        }
        #endregion
    }
}
