using UnityEngine;

namespace BGJ_14
{
    public class Scrap : MonoBehaviour
    {
        [SerializeField] private int _minGearsAmount;
        [SerializeField] private int _maxGearsAmount;

        [SerializeField][Range(0, 1)] private float _activationProbability;

        private int _currentGearAmount;

        private void Awake()
        {
            Deactivate();
        }

        public void Activate()
        {
            gameObject.SetActive(Random.value < _activationProbability);
            _currentGearAmount = Mathf.CeilToInt(Mathf.Lerp(
                _minGearsAmount,
                _maxGearsAmount,
                Random.value));
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
