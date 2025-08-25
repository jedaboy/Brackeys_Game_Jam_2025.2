using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BGJ_14
{
    public class SentinelSpawner : MonoBehaviour
    {
        private Transform _myTransform;
        [SerializeField] private GameObject _sentinelPrefab;
        private List<GameObject> _spawnedSentinels = new List<GameObject>();

        [SerializeField] private int _sentinelCount = 10;
        [SerializeField] private float _spawnAreaRadius = 1;

        private bool _sentinelsActive;
        [SerializeField] private float _respawnDeadSentinelTime = 10f;
        private float _respawnDeadSentinelTimeCounter;
        [SerializeField] private Transform[] _deadSentinelRespawnPoints;

        private void Awake()
        {
            _myTransform = transform;
        }

        private void Update()
        {
            if (!_sentinelsActive)
                return;

            DeadSentinelRespawnLogic();
        }

        public void SpawnSentinels()
        {
            for (int i = 0; i < _sentinelCount; i++)
            {
                Vector3 position = _myTransform.position +
                    (Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.forward * Random.Range(0, _spawnAreaRadius));

                GameObject sentinel = ObjectPoolManager.instance.InstantiateInPool(
                    _sentinelPrefab,
                    position,
                    Quaternion.identity);

                _spawnedSentinels.Add(sentinel);
            }

            _sentinelsActive = true;
        }

        public void DeleteSentinels()
        {
            foreach (GameObject sentinel in _spawnedSentinels)
                sentinel.SetActive(false);

            _spawnedSentinels.Clear();

            _sentinelsActive = false;
        }

        private void DeadSentinelRespawnLogic()
        {
            _respawnDeadSentinelTimeCounter -= Time.deltaTime;
            if (_respawnDeadSentinelTimeCounter <= 0)
            {
                _respawnDeadSentinelTimeCounter = _respawnDeadSentinelTime;

                foreach (GameObject sentinel in _spawnedSentinels)
                {
                    if (sentinel.activeInHierarchy)
                        continue;

                    Transform respawnPoint = _deadSentinelRespawnPoints[Random.Range(0, _deadSentinelRespawnPoints.Length)];
                    ObjectPoolManager.instance.InstantiateInPool(
                        _sentinelPrefab,
                        respawnPoint.position,
                        respawnPoint.rotation);
                    break;
                }
            }
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
