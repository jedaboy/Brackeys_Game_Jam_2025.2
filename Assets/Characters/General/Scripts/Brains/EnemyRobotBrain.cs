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
        public float radius = 5f;
        public LayerMask layerMask; // opcional, para filtrar apenas certos objetos
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
        protected override void StopMovement()
        {
            enemyRobotController.Stop();
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
            // Atira
            enemyRobotController.Shoot();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Safe Zone"))
            {
                gameObject.SetActive(false);
                if (fsmManager != null)
                    fsmManager.SetTrigger("IsDeadT");
                
            }
        }
        private void CallHelp()
        {
            // Cria uma LayerMask para a layer 7
            int layerMask = 1 << 7;

            // Cria uma esfera de detecção ao redor do objeto
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask);

            foreach (Collider col in hitColliders)
            {
                GameObject enemy = col.gameObject;
                enemy.GetComponent<EnemyRobotBrain>().detectionRange = 80;
            }
        }
        private void FaceTarget()
        {
            if (target == null) return;

            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);

            targetPositionReference.position = target.position;
        }

        protected override void Update()
        {

            if (battery.CurrentCharge < battery.initialCharge && battery.isSentinelShooting == false)
            {
                Debug.Log("Estou levando tiro, me ajudem");
                detectionRange = 80;
                CallHelp();
            }
            if ((bool)fsmManager.GetParameterValue("Dying") == false)
            {
                FaceTarget();
            }

            if (battery.IsEmpty)
            {
                fsmManager.SetTrigger("IsDeadT");
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

            if (battery.CurrentCharge < battery.initialCharge && battery.isSentinelShooting == false)
            {
                detectionRange = 80;
                CallHelp();
            }

            base.Update();
        }
    }
}
