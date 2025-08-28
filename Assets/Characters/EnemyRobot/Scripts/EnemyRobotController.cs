using UnityEngine;
using UnityEngine.AI;
using GRD.FSM;

namespace BGJ14
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyRobotController : MonoBehaviour
    {
        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            // Para inimigos no chão, o NavMeshAgent já controla a posição e altura normalmente
            agent.updatePosition = true;
            agent.updateRotation = true;
        }

        /// <summary>
        /// Faz o robô se mover para um destino específico no NavMesh.
        /// </summary>
        public void MoveTo(Vector3 destination)
        {
            if (agent != null && agent.isOnNavMesh)
                agent.SetDestination(destination);
        }

        /// <summary>
        /// Para completamente o movimento do robô.
        /// </summary>
        public void Stop()
        {
            if (agent != null)
                agent.isStopped = true;
        }

        /// <summary>
        /// Retoma o movimento após ter sido parado.
        /// </summary>
        public void Resume()
        {
            if (agent != null)
                agent.isStopped = false;
        }
    }
}
