using GRD.FSM;
using System;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;


namespace BGJ14
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RobotController : CharacterController
    {
        public PlayerInputController robotIC;
        public float weight;
        public float velocity;
        public bool gearsAffectWeight;

        public int ammo;
        public Camera m_Cam;
        private Vector3 moveInput;
        public GameObject robotArm;
        [SerializeField] Transform vfxExplosion;


        [SerializeField] private float distance = 2f;
        [SerializeField] private float sensitivity = 3f;
        [SerializeField] private float minY = -10f;
        [SerializeField] private float maxY = 80f;


        private float yaw;  
        private float pitch; 


        public void Update()
        { 
  
            CamMove();
            ShootInput();
        }

        public void FixedUpdate()
        {
            Debug.Log(ChecKGroundStatus());
            if(ChecKGroundStatus())
            Move();          
            battery.DrainOverTime();

        }

        public void MoveInput()
        {
            // Calcula dire��o de movimento relativa � c�mera
            Vector3 moveDir = robotIC.move.x * m_Cam.transform.right
                            + robotIC.move.y * Vector3.ProjectOnPlane(m_Cam.transform.forward, Vector3.up).normalized;

            if (robotIC.sprint)
            {
                moveDir *= 3f;
                battery.drainRate = 2f;
            }
            else
                battery.drainRate = 0.1f;

            float fowardAmount = Mathf.Abs(moveDir.z);

            anim.SetFloat("Running", fowardAmount);
            anim.SetFloat("MovingSpeed", fowardAmount);


            moveInput = moveDir;

            // Se houver movimento, rotaciona corpo para dire��o
            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }

        public void JumpInput()
        {
            if (robotIC.jump)
            {
                fsmManager.SetBool("Jump", true);
            }
        }

        public void CancelJumpInput()
        {
            fsmManager.SetBool("Jump", false);
        }
        public void DestroyCharacter()
        {
            Instantiate(vfxExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
        private void Move()
        {
            GetComponent<Rigidbody>().velocity = new Vector3(moveInput.x, GetComponent<Rigidbody>().velocity.y, moveInput.z);
         
        }
        private void ShootInput()
        {
            if (robotIC.shoot)
            {
                Shoot();
            }
        }
        public void CamMove()
        {
            // --- Input da câmera ---
            yaw += robotIC.camMove.x * sensitivity;
            pitch -= robotIC.camMove.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minY, maxY);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            // offset configurável (X = lado, Y = altura, Z = distância atrás)
            Vector3 cameraOffset = new Vector3(3f, 3f, -distance);

            // Calcula posição da câmera
            Vector3 desiredPosition = transform.position + Vector3.up * 0.5f + rotation * new Vector3(0f, 2f, -distance);
            m_Cam.transform.position = desiredPosition;

            // Colisão com paredes
            RaycastHit hit;
            Vector3 finalPosition = desiredPosition;
            if (Physics.Linecast(transform.position + Vector3.up * 0.5f, desiredPosition, out hit))
            {
                finalPosition = hit.point + hit.normal * 0.2f;
            }
            m_Cam.transform.position = finalPosition;

            // Rotação da câmera olhando para o player
            m_Cam.transform.rotation = Quaternion.LookRotation(
                (transform.position + Vector3.up * 0.5f) - m_Cam.transform.position,
                Vector3.up
            );

            // --- Rotação do braço seguindo o mouse ---
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            Vector3 aimPoint = ray.GetPoint(50f); // Ponto distante (50 unidades à frente)
            Vector3 aimDir = (aimPoint - robotArm.transform.position).normalized;

            // Rotação desejada do braço
            Quaternion targetArmRot = Quaternion.LookRotation(aimDir, Vector3.up);

            // ---- Limitação da rotação do braço ----
            float maxAngle = 60f;
            float angle = Quaternion.Angle(transform.rotation, targetArmRot);

            if (angle > maxAngle)
            {
                targetArmRot = Quaternion.RotateTowards(transform.rotation, targetArmRot, maxAngle);
            }

            // Aplica no braço
            robotArm.transform.rotation = targetArmRot;

            // --- Rotação do corpo só quando atirando ---
            if (robotIC.shoot)
            {
                transform.rotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(aimDir, Vector3.up),
                    Vector3.up
                );
            }

        }
        public bool ChecKGroundStatus()
        {
            float radius = 0.5f;
            float distance = 0.6f;
            int ignoreLayer = 6; int layerMask = ~(1 << ignoreLayer);

            bool grounded = Physics.CheckSphere(
                transform.position + Vector3.down * distance,
                radius,
                layerMask
                );
            anim.SetBool("OnGround", grounded);
            return grounded;
        }

        void OnDrawGizmosSelected()
        {
            // Cor  esfera
            Gizmos.color = Color.red;
            float radius = 0.5f;
            float distance = 0.6f;
            Vector3 pos = transform.position + Vector3.down * distance;

            // Desenha a esfera
            Gizmos.DrawWireSphere(pos, radius);
        }
    }
}