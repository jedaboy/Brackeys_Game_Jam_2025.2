using BGJ14;
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

        public List<EnemyRobotController> SpawnEnemies(GameObject enemyPrefab,
            int maxEnemyCount, int minEnemyLevel, int maxEnemyLevel)
        {
            List<EnemyRobotController> instantiatedEnemies = new List<EnemyRobotController>();

            int enemyCount = Mathf.CeilToInt(maxEnemyCount * _enemyDensity);
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 position = _myTransform.position +
                    (Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.forward * Random.Range(0, _spawnAreaRadius));

                EnemyRobotController instantiatedEnemy =
                    ObjectPoolManager.instance.InstantiateInPool(
                        enemyPrefab,
                        position,
                        Quaternion.Euler(0, Random.Range(0, 360), 0))
                    .GetComponent<EnemyRobotController>();

                int minLevel = Mathf.CeilToInt(Mathf.Lerp(minEnemyLevel, maxEnemyLevel, _minEnemyLevel));
                int maxLevel = Mathf.CeilToInt(Mathf.Lerp(minEnemyLevel, maxEnemyLevel, _maxEnemyLevel));
                int level = Mathf.CeilToInt(Mathf.Lerp(minLevel, maxLevel, Random.value));
                instantiatedEnemy.Setup(level);

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
