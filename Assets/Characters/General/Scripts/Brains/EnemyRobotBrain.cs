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
        }

        protected override void MoveTo(Vector3 position)
        {
            if (enemyRobotController != null)
                enemyRobotController.MoveTo(position);
        }

        protected override void Patrol()
        {
            if (fsmManager != null)
                fsmManager.SetBool("Target", false);
        }

        protected override Transform FindTarget()
        {
            // Sempre prioriza Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
                return player.transform;

            return null;
        }

        protected override void SetAttack(bool attacking)
        {
            base.SetAttack(attacking);
            if (fsmManager != null)
                fsmManager.SetBool("Target", attacking);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Safe Zone"))
            {
                // Seta o state Died na FSM
                if (fsmManager != null)
                    fsmManager.SetBool("Dead", true);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (battery.IsEmpty)
            {
                Debug.Log("Morto");
                fsmManager.SetBool("Dead", true);
            }
            // Se a vida estiver baixa, ativa Flee
            else if (battery != null && battery.CurrentCharge / battery.maxCharge <= 0.25f)
            {
                if (fsmManager != null)
                    fsmManager.SetBool("Dying", true); // bool da FSM que ativa Flee
            }
            else
            {
                if (fsmManager != null)
                    fsmManager.SetBool("Dying", false);
            }
        }
    }
}
