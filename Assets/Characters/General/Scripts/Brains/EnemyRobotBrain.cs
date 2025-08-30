using UnityEngine;
using GRD.FSM;

namespace BGJ14
{
    [RequireComponent(typeof(EnemyRobotInputController))]
    [RequireComponent(typeof(Battery))]
    public class EnemyRobotBrain : AIBrain
    {
        public FSM_Manager fsmManager;
        public EnemyRobotController enemyRobotController;

        private Battery battery;

        protected override void Awake()
        {
            base.Awake();
            battery = GetComponent<Battery>();
            useFieldOfView = false;
        }

        protected override void MoveTo(Vector3 position)
        {
            if (enemyRobotController != null)
                enemyRobotController.MoveTo(position);
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

            return closest;
        }

        protected override void DetectTarget()
        {
            target = FindTarget(); // usa a nossa lógica do SentinelBrain
        }

        protected override void SetAttack(bool attacking)
        {
            base.SetAttack(attacking);
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
            enemyRobotController.Shoot();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Safe Zone"))
            {
                if (fsmManager != null)
                    fsmManager.SetBool("IsDead", true);
            }
        }

        protected override void Update()
        {
            if (battery.IsEmpty)
            {
                fsmManager.SetBool("IsDead", true);
            }
            else if (battery != null && battery.CurrentCharge / battery.maxCharge <= 0.25f)
            {
                // ativa estado Flee
                if (fsmManager != null)
                    fsmManager.SetBool("Dying", true);
                SetAttack(false);
                return;
            }
            else
            {
                if (fsmManager != null)
                    fsmManager.SetBool("Dying", false);
            }

            base.Update();
        }
    }
}
