using UnityEngine;

namespace BGJ14
{
    public abstract class AIBrain : MonoBehaviour
    {
        protected SentinelInputController inputController;

        protected virtual void Awake()
        {
            inputController = GetComponent<SentinelInputController>();
        }

        // Cada brain vai implementar sua lógica
        public abstract void Think();
    }
}
