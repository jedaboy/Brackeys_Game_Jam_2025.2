using UnityEngine;
using UnityEngine.AI;
using GRD.FSM;

namespace BGJ14
{
    [RequireComponent(typeof(SentinelInputController))]
    public class SentinelBrain : AIBrain
    {
        public FSM_Manager fsmManager;
        public SentinelController sentinelController;

        [Header("Patrulha")]
        public float patrolRadius = 20f;
        private float patrolTimer = 0f;
        private Battery battery;

        protected override void Awake()
        {
            base.Awake();
            battery = GetComponent<Battery>();
            MoveToRandomPatrolPoint();
        }

        protected override void MoveTo(Vector3 position)
        {
            if (sentinelController != null)
                sentinelController.MoveTo(position);
        }

        protected override void Patrol()
        {
            if (fsmManager != null)
                fsmManager.SetBool("Target", false);

            if (sentinelInputController != null)
                sentinelInputController.SetPursuit(false);

            patrolTimer += Time.deltaTime;
            if (patrolTimer >= 4f)
            {
                patrolTimer = 0f;
                MoveToRandomPatrolPoint();
            }
        }

        protected override void SetAttack(bool attacking)
        {
            base.SetAttack(attacking); // chama SetPursuit(true/false) via AIBrain
            if (fsmManager != null)
                fsmManager.SetBool("Target", attacking);
        }

        // Usa o AttackTarget do AIBrain (instancia o projétil apontando pro target).
        // Se quiser customizar, descomente e sobrescreva aqui.

        private void MoveToRandomPatrolPoint()
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, patrolRadius);
            if (randomPoint != Vector3.zero)
                MoveTo(randomPoint);
        }

        protected override Transform FindTarget()
        {
            Transform closest = null;
            float minDist = detectionRange;

            // Busca Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist <= minDist)
                {
                    closest = player.transform;
                    minDist = dist;
                }
            }

            // Busca inimigos
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemies)
            {
                float dist = Vector3.Distance(transform.position, e.transform.position);
                if (dist <= minDist)
                {
                    closest = e.transform;
                    minDist = dist;
                }
            }

            return closest;
        }


        private Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius)
        {
            Vector3 randomDir = Random.insideUnitSphere * radius + center;
            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, radius, NavMesh.AllAreas))
                return hit.position;
            return Vector3.zero;
        }

        protected override void Update()
        {
            base.Update();
            
            if (battery.IsEmpty)
            fsmManager.SetBool("Dead", true);
            
        }
    }
}


