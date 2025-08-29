using System.Collections.Generic;
using UnityEngine;

namespace BGJ_14
{
    public class ExpeditionManager : MonoBehaviour
    {
        private static ExpeditionManager _instance;
        public static ExpeditionManager instance => _instance;

        private PlayerProgress _playerProgress;

        [SerializeField] private ExpeditionProperties _expeditionProperties;
        private int _currentExpeditionNumber;

        private bool _expeditionRunning;
        private float _expeditionTimeCounter;
        [SerializeField] private float _sentinelsTime;

        [SerializeField] private GameObject _robotEnemyPrefab;
        [SerializeField] private SentinelSpawner _sentinelSpawner;

        [SerializeField] private EnemySpawnBase[] _enemySpawnBases;
        [SerializeField] private Scrap[] _scraps;

        private List<GameObject> _spawnedRobotEnemies = new List<GameObject>();

        public bool expeditionRunning => _expeditionRunning;

        private void Awake()
        {
            _instance = this;

            _playerProgress = GameManager.instance.GetService<GameSessionService>().
                playerProgress;
        }

        private void Update()
        {
            if (!_expeditionRunning)
                return;

            if (_expeditionTimeCounter < _sentinelsTime)
            {
                _expeditionTimeCounter += Time.deltaTime;
                if (_expeditionTimeCounter >= _sentinelsTime)
                {
                    ReleaseSentinels();
                }
            }
        }

        [EButton]
        public void StartExpedition()
        {
            if (_expeditionRunning)
                return;

            _playerProgress.OnExpeditionStart();

            _currentExpeditionNumber++;

            SpawnRobotEnenies();
            SpawnScrap();
            _expeditionTimeCounter = 0;
            _expeditionRunning = true;
        }

        private void ReleaseSentinels()
        {
            _sentinelSpawner.SpawnSentinels();
        }

        [EButton]
        public void EndExpedition()
        {
            if (!_expeditionRunning)
                return;

            _playerProgress.OnExpeditionEnd();

            _expeditionRunning = false;
            DeleteRobotEnemies();
            DeleteScrap();
            _sentinelSpawner.DeleteSentinels();
        }

        private void SpawnRobotEnenies()
        {
            foreach (EnemySpawnBase enemyBase in _enemySpawnBases)
            {
                List<GameObject> instantiatedEnemies = enemyBase.SpawnEnemies(_robotEnemyPrefab,
                    _expeditionProperties.GetEnemiesPerSpawnBase(_currentExpeditionNumber),
                    _expeditionProperties.GetEnemyMinLevel(_currentExpeditionNumber),
                    _expeditionProperties.GetEnemyMaxLevel(_currentExpeditionNumber));

                _spawnedRobotEnemies.AddRange(instantiatedEnemies);
            }
        }

        private void SpawnScrap()
        {
            foreach (Scrap scrap in _scraps)
            {
                scrap.Activate();
            }
        }

        private void DeleteRobotEnemies()
        {
            foreach (GameObject robot in _spawnedRobotEnemies)
                robot.SetActive(false);

            _spawnedRobotEnemies.Clear();
        }

        private void DeleteScrap()
        {
            foreach (Scrap scrap in _scraps)
            {
                scrap.Deactivate();
            }
        }
    }
}
