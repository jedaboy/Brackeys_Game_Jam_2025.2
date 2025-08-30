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
            useFieldOfView = true;
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
        protected override void AttackTarget()
        {
            if (target == null) return;

            // Gira a sentinela para mirar
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);

            // Atira
            sentinelController.Shoot();
        }

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

        protected override void DetectTarget()
        {
            target = FindTarget(); // usa a nossa lógica do SentinelBrain
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
            fsmManager.SetBool("IsDead", true);
            
        }
    }
}


