using UnityEngine;

namespace BGJ14
{
    public abstract class AIBrain : MonoBehaviour
    {
        [Header("Detection & Combat")]
        public float detectionRange = 10f;
        public float stopDistance = 5f;
        public float fireRate = 0.5f;
        protected float fireTimer = 0f;
        public float bulletDamage = 10f; // dano do tiro

        [SerializeField] protected Transform bulletProjectile;
        [SerializeField] protected Transform spawnBulletPosition;
        [SerializeField] protected Transform targetPositionReference;
        [SerializeField] protected bool useFieldOfView = true;
        [SerializeField] protected float fieldOfView = 90f;
        [SerializeField] protected LayerMask targetMask;
        [SerializeField] protected LayerMask obstacleMask;

        protected Transform target;

        protected SentinelInputController sentinelInputController;
        protected EnemyRobotInputController enemyRobotInputController;

        protected virtual void Awake()
        {
            sentinelInputController = GetComponent<SentinelInputController>();
            enemyRobotInputController = GetComponent<EnemyRobotInputController>();
        }

        protected virtual void Update()
        {
            DetectTarget();
            Think();
        }

// dentro de AIBrain

        public virtual void Think()
        {
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                // mover até uma distância segura
                if (dist > stopDistance)
                    MoveTo(target.position);
                else
                    StopMovement(); // <-- foi MoveTo(transform.position); agora para o movimento

                // atacar se dentro do range
                if (dist <= detectionRange)
                {
                    SetAttack(true);

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
                SetAttack(false);
                Patrol();
            }
        }

        // novo método para subclasses sobrescreverem
        protected virtual void StopMovement()
        {
            // por padrão não faz nada; subclasses (com NavMeshAgent) sobrescrevem
        }


       protected virtual void AttackTarget()
        {
            if (target == null || bulletProjectile == null || spawnBulletPosition == null) return;


        }


        protected virtual Transform FindTarget()
        {
            return null; // por padrão não retorna ninguém
        }

        protected virtual void DetectTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, targetMask);
            foreach (var col in colliders)
            {
                Transform candidate = col.transform;
                Vector3 dirToTarget = (candidate.position - transform.position).normalized;

                // Aplica o cone de visão só se useFieldOfView == true
                if (useFieldOfView)
                {
                    float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
                    if (angleToTarget > fieldOfView * 0.5f) 
                        continue;
                }

                // Checa se não tem obstáculos no meio
                if (!Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, detectionRange, obstacleMask))
                {
                    target = candidate;
                    return;
                }
            }

            target = null;
        }


        protected void ClearTarget()
        {
            target = null;
            SetAttack(false);
        }

        // --------------------------
        // Métodos que subclasses podem sobrescrever
        // --------------------------
        protected virtual void MoveTo(Vector3 position) { }
        protected virtual void Patrol() { }
        protected virtual void SetAttack(bool attacking)
        {
            if (enemyRobotInputController != null)
                enemyRobotInputController.SetAttack(attacking);
            if (sentinelInputController != null)
                sentinelInputController.SetPursuit(attacking);
        }
    }
}
