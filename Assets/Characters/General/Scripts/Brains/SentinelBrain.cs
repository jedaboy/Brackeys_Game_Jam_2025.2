using UnityEngine;
using UnityEngine.AI;
using GRD.FSM;

namespace BGJ14
{
    [RequireComponent(typeof(SentinelInputController))]
    public class SentinelBrain : AIBrain
    {
        private Transform target;
        public FSM_Manager fsmManager;
        public SentinelController sentinelController;

        [Header("Movimento")]
        public float patrolRadius = 20f;
        public float stopPursuitDistance = 12f;

        [Header("Ataque")]
        public float damagePerShot = 10f;
        public float fireRate = 0.5f;
        private float fireTimer = 0f;

        private float patrolTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, patrolRadius);
            if (randomPoint != Vector3.zero)
                sentinelController.MoveTo(randomPoint);
        }

        private void Update()
        {
            Think();
        }

        public override void Think()
        {
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                if (dist > stopPursuitDistance)
                {
                    ClearTarget();
                }
                else
                {
                    // Perseguição
                    fsmManager.SetBool("Target", true);
                    inputController.SetPursuit(true);

                    sentinelController.MoveTo(target.position);

                    // Ataque
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= fireRate)
                    {
                        fireTimer = 0f;
                        AttackTarget();
                    }
                }
            }
            else
            {
                // Patrulha
                fsmManager.SetBool("Target", false);
                inputController.SetPursuit(false);

                patrolTimer += Time.deltaTime;
                if (patrolTimer >= 4f)
                {
                    patrolTimer = 0f;
                    Vector3 randomPoint = GetRandomPointOnNavMesh(sentinelController.transform.position, patrolRadius);
                    if (randomPoint != Vector3.zero)
                    {
                        sentinelController.MoveTo(randomPoint);
                    }
                }
            }
        }

        private void AttackTarget()
        {
            if (target == null) return;

            Battery battery = target.GetComponent<Battery>();
            if (battery != null)
            {
                battery.Drain(damagePerShot);

                if (battery.IsEmpty)
                {
                    Destroy(target.gameObject);
                    ClearTarget();
                }
            }
        }

        public void ClearTarget()
        {
            target = null;
            fsmManager.SetBool("Target", false);
            inputController.SetPursuit(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Target"))
            {
                target = other.transform;
                fsmManager.SetBool("Target", true);
                inputController.SetPursuit(true);
            }
        }

        private Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius)
        {
            Vector3 randomDir = Random.insideUnitSphere * radius;
            randomDir += center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return Vector3.zero;
        }
    }
}
