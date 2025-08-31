using UnityEngine;

namespace BGJ_14
{
    public class Scrap : MonoBehaviour
    {
        [SerializeField] private int _minGearsAmount;
        [SerializeField] private int _maxGearsAmount;

        [SerializeField][Range(0, 1)] private float _activationProbability;

        [SerializeField] private GameObject gearPrefab; // Prefab do Gear

        private int _currentGearAmount;

        private void Awake()
        {
            //
        }

        public void Activate()
        {
            if (Random.value < _activationProbability)
            {
                _currentGearAmount = Mathf.CeilToInt(Mathf.Lerp(
                    _minGearsAmount,
                    _maxGearsAmount,
                    Random.value));

                DropGears(_currentGearAmount);
            }
            else
            {
                // Não dropa nada, mas também não some o objeto
            }
        }


        private void DropGears(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0.5f,
                    Random.Range(-0.5f, 0.5f)
                );

                Transform gear = ObjectPoolManager.instance.InstantiateInPool(
                    gearPrefab,
                    spawnPos,
                    Quaternion.identity
                ).transform;

                Rigidbody rb = gear.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
                    rb.AddForce(randomDir * 3f, ForceMode.Impulse);
                }
            }
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
