using UnityEngine;
using UnityEngine.AI;
using GRD.FSM;

namespace BGJ14
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossController : CharacterController
    {
        private NavMeshAgent agent;
        [SerializeField] Transform vfxExplosion;

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
            {
                agent.isStopped = false;   // reativa movimento/rotação do NavMeshAgent
                agent.SetDestination(destination);
            }
        }

        public void Stop()
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath(); // evita pequenas correções de rotação baseadas no path
            }
        }



        public void DestroyCharacter()
        {
            Instantiate(vfxExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }

        public void SpawnSentinels()
        {
            Debug.Log("Criou sentinelas");
        }
        public void Resume()
        {
            if (agent != null)
                agent.isStopped = false;
        }
    }
}
