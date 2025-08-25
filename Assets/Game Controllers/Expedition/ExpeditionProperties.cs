using UnityEngine;

namespace BGJ_14
{
    [CreateAssetMenu(fileName = "Expedition Properties", menuName = "Expedition/Expedition Properties")]
    public class ExpeditionProperties : ScriptableObject
    {
        [SerializeField] private int[] _enemiesPerSpawnBaseByExpedition;
        [SerializeField] private int[] _enemyMinLevelByExpedition;
        [SerializeField] private int[] _enemyMaxLevelByExpedition;

        public int GetEnemiesPerSpawnBase(int expeditionNumber)
        {
            return _enemiesPerSpawnBaseByExpedition[Mathf.Min(
                expeditionNumber - 1, _enemiesPerSpawnBaseByExpedition.Length - 1)];
        }

        public int GetEnemyMinLevel(int expeditionNumber)
        {
            return _enemyMinLevelByExpedition[Mathf.Min(
                expeditionNumber - 1, _enemyMinLevelByExpedition.Length - 1)];
        }

        public int GetEnemyMaxLevel(int expeditionNumber)
        {
            return _enemyMaxLevelByExpedition[Mathf.Min(
                expeditionNumber - 1, _enemyMaxLevelByExpedition.Length - 1)];
        }
    }
}
