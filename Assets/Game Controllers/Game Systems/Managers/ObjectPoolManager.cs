using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager instance 
    {
        get 
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<ObjectPoolManager>();
            return _instance;
        }
    }
    
    private Dictionary<GameObject, List<GameObject>> _objectPools = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (_instance != null && _instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        SceneManager.activeSceneChanged += OnChangeScene;
    }

    public GameObject InstantiateInPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (_objectPools.ContainsKey(prefab))
        {
            GameObject newObject = GetGameObjectInPool(_objectPools[prefab]);
            if (newObject == null)
            {
                newObject = Instantiate(prefab, position, rotation);
                _objectPools[prefab].Add(newObject);
                return newObject;
            }

            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.SetActive(true);
            return newObject;
        }

        CreateObjectPool(prefab);
        GameObject objectToInstantiate = Instantiate(prefab, position, rotation);
        _objectPools[prefab].Add(objectToInstantiate);
        return objectToInstantiate;
    }

    private GameObject GetGameObjectInPool(List<GameObject> pool)
    {
        GameObject availabelObject = pool.FirstOrDefault(go => go.activeInHierarchy == false);
        return availabelObject;
    }

    private void CreateObjectPool(GameObject prefab)
    {
        _objectPools.Add(prefab, new List<GameObject>());
    }

    private void OnChangeScene(Scene current, Scene next) 
    {
        _objectPools.Clear();
    }
}
