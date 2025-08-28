using UnityEngine;
using UnityEngine.AI;

namespace BGJ14
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SentinelController : MonoBehaviour
    {
        public float hoverHeight = 5f;
        public LayerMask groundMask;

        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false; // importante: vamos controlar a posição manualmente
        }

        private void FixedUpdate()
        {
            UpdateHover();
        }

        private void UpdateHover()
        {
            if (agent == null) return;

            Vector3 nextPos = agent.nextPosition;

            // Raycast para determinar altura do terreno
            if (Physics.Raycast(nextPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, groundMask))
            {
                nextPos.y = hit.point.y + hoverHeight;
            }

            // Aplica posição ajustada
            transform.position = nextPos;

            // Sincroniza agente com o transform
            agent.nextPosition = transform.position;
        }

        public void MoveTo(Vector3 destination)
        {
            if (agent != null)
                agent.SetDestination(destination);
        }
    }
}
