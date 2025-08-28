using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BGJ_14
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager instance => _instance;

        private Dictionary<Type, IService> _services;

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
            InstallServices();
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

        #region Services
        private void InstallServices() 
        {
            _services = new Dictionary<Type, IService>();

            _services.Add(typeof(GameSessionService), new GameSessionService());
        }

        public T GetService<T>()
        {
            return (T)_services[typeof(T)];
        }
        #endregion
    }
}
