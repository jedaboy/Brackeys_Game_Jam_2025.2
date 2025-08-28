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

        public virtual void Think()
        {
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                // mover até uma distância segura
                if (dist > stopDistance)
                    MoveTo(target.position);
                else
                    MoveTo(transform.position); // fica parado, não cola no alvo

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

       protected virtual void AttackTarget()
        {
            if (target == null || bulletProjectile == null || spawnBulletPosition == null) return;

            // direção até o alvo
            Vector3 aimDir = (target.position - spawnBulletPosition.position).normalized;

            Transform bullet = Instantiate(
                bulletProjectile,
                spawnBulletPosition.position,
                Quaternion.LookRotation(aimDir, Vector3.up)
            );

            // ignora colisão com o próprio corpo
            Collider bulletCol = bullet.GetComponent<Collider>();
            Collider selfCol = GetComponent<Collider>();
            if (bulletCol != null && selfCol != null)
                Physics.IgnoreCollision(bulletCol, selfCol);

            // verifica se o alvo tem Battery
            Battery battery = target.GetComponent<Battery>();
            if (battery != null)
            {
                battery.Drain(bulletDamage);
            }
        }


        protected virtual Transform FindTarget()
        {
            return null; // por padrão não retorna ninguém
        }

        protected virtual void DetectTarget()
        {
            target = null;

            // Primeiro verifica players
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                if (p == this.gameObject) continue; // não atacar a si mesmo
                if (Vector3.Distance(transform.position, p.transform.position) <= detectionRange)
                {
                    target = p.transform;
                    return;
                }
            }

            // Se for sentinela, verifica inimigos (tag "Enemy"), exceto ela mesma
            if (sentinelInputController != null)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var e in enemies)
                {
                    if (e == this.gameObject) continue; // não atacar a si mesma
                    if (Vector3.Distance(transform.position, e.transform.position) <= detectionRange)
                    {
                        target = e.transform;
                        return;
                    }
                }
            }
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
