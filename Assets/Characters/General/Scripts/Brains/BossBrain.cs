using UnityEngine;
using GRD.FSM;

namespace BGJ14
{
    [RequireComponent(typeof(EnemyRobotInputController))]
    [RequireComponent(typeof(Battery))]
    public class BossBrain : AIBrain
    {
        public FSM_Manager fsmManager;
        public BossController bossController;

        private Battery battery;

        // Flags de checkpoints
        private bool invoked75 = false;
        private bool invoked50 = false;
        private bool invoked25 = false;

        protected override void Awake()
        {
            base.Awake();
            battery = GetComponent<Battery>();
            useFieldOfView = false;
        }

        protected override void MoveTo(Vector3 position)
        {
            if (bossController != null)
                bossController.MoveTo(position);
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
            target = FindTarget();
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
            bossController.Shoot();
        }

        protected override void Update()
        {
            if (battery.IsEmpty)
            {
                fsmManager.SetBool("IsDead", true);
            }
            else
            {
                float healthPercent = battery.CurrentCharge / battery.maxCharge;

                // Checkpoints de invocação
                if (!invoked75 && healthPercent <= 0.75f)
                {
                    invoked75 = true;
                    SpawnSentinels();
                }
                if (!invoked50 && healthPercent <= 0.50f)
                {
                    invoked50 = true;
                    SpawnSentinels();
                }
                if (!invoked25 && healthPercent <= 0.25f)
                {
                    invoked25 = true;
                    SpawnSentinels();
                }
            }

            base.Update();
        }

        private void SpawnSentinels()
        {
            bossController.SpawnSentinels();
        }
    }
}
