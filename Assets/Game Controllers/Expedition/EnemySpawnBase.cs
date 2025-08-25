using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BGJ_14
{
    public class EnemySpawnBase : MonoBehaviour
    {
        private Transform _myTransform;

        [SerializeField][Range(0, 1)] private float _minEnemyLevel;
        [SerializeField][Range(0, 1)] private float _maxEnemyLevel;
        [SerializeField][Range(0, 1)] private float _enemyDensity = 1;

        [SerializeField] private float _spawnAreaRadius = 1;

        private void Awake()
        {
            _myTransform = transform;
        }

        public List<GameObject> SpawnEnemies(GameObject enemyPrefab,
            int maxEnemyCount, int minEnemyLevel, int maxEnemyLevel)
        {
            List<GameObject> instantiatedEnemies = new List<GameObject>();

            int enemyCount = Mathf.CeilToInt(maxEnemyCount * _enemyDensity);
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 position = _myTransform.position +
                    (Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.forward * Random.Range(0, _spawnAreaRadius));

                GameObject instantiatedEnemy =
                    ObjectPoolManager.instance.InstantiateInPool(
                        enemyPrefab,
                        position,
                        Quaternion.Euler(0, Random.Range(0, 360), 0));

                instantiatedEnemies.Add(instantiatedEnemy);
            }

            return instantiatedEnemies;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Selection.activeGameObject != gameObject)
                return;

            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, _spawnAreaRadius);
        }

#endif
    }
}
