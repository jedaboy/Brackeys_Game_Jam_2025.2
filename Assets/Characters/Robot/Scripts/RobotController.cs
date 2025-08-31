using System;
using System.Collections;
using UnityEngine;


namespace BGJ14
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RobotController : CharacterController
    {
        public PlayerInputController robotIC;
        public float weight;
        public float currentVelocity;
        public float maxVelocity;
        public float minVelocity;
        public float timeToCollect = 3f;
        public bool isOverWeight;
        private UnityEngine.Animations.Rigging.Rig armAimRig;
        private UnityEngine.Animations.Rigging.Rig headAimRig;
        public int ammo;
        public Camera m_Cam;
        private Vector3 moveInput;
        public GameObject robotArm;
        public Action<int> OnCollectGear;
        public Func<int, bool> OnAmmoUpdate;
        public Func<bool> OnLBUpdate;
        public Func<int>  OnGetGears;
        public Func<int,bool> OnDropGears;

        [SerializeField] Transform vfxExplosion;
        [SerializeField] Transform vfxHeal;

        [SerializeField] public float fireRate = 0.75f; // Tempo entre tiros (em segundos)
        private float lastShootTime;

        [SerializeField] private float distance = 2f;
        [SerializeField] private float sensitivity = 3f;
        [SerializeField] private float minY = -10f;
        [SerializeField] private float maxY = 80f;

        [HideInInspector]
        public bool IsStoreOpen = false;

        private bool canDrop = true; // Controle de cooldown do drop

        private float yaw;
        private float pitch;


        public void Update()
        {

            CamMove();
            ShootInput();
            HealInput();
            DropInput();
        }

        public void FixedUpdate()
        {
            if (ChecKGroundStatus())
                Move();
            battery.DrainOverTime();
            handleWeight();

        }

        public bool CanReceiveInput()
        {
            if (IsStoreOpen)
                return false;

            return true;
        }

        public void MoveInput()
        {
            if (CanReceiveInput() == false)
            {
                moveInput = Vector3.zero;
                anim.SetFloat("Running", 0);
                anim.SetFloat("MovingSpeed", 0);

                anim.SetFloat("ForwardMoveSpeed", 0);
                anim.SetFloat("RightMoveSpeed", 0);

                return;
            }
            if (isOverWeight)
                currentVelocity = 1f;
            // Calcula dire��o de movimento relativa � c�mera
            Vector3 moveDir = (robotIC.move.x * currentVelocity) * m_Cam.transform.right
                            + (robotIC.move.y * currentVelocity) * Vector3.ProjectOnPlane(m_Cam.transform.forward, Vector3.up).normalized;

            if (robotIC.sprint && weight < 70f)
            {
                moveDir *= 2f;
                battery.drainRate = 2f;
            }
            else
                battery.drainRate = 0.3f;

            float movementAmount = moveDir.magnitude; // Magnitude pega frente+lado



            moveInput = moveDir;
            anim.SetFloat("MovingSpeed", movementAmount);

            // Se houver movimento, rotaciona corpo para dire��o
            if (moveDir.sqrMagnitude > 0.001f && !robotIC.shoot)
            {
                anim.SetFloat("Running", movementAmount);


                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
            else if (robotIC.shoot)
            {
                // Converte moveDir para o espaço local do jogador
                Vector3 localMove = transform.InverseTransformDirection(moveDir);

                // LocalMove.z => frente/tras (ForwardMoveSpeed)
                // LocalMove.x => lateral (RightMoveSpeed)
                anim.SetFloat("ForwardMoveSpeed", localMove.z);
                anim.SetFloat("RightMoveSpeed", localMove.x);
            }
        }

        public void JumpInput()
        {
            if (CanReceiveInput() == false)
                return;
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
            ObjectPoolManager.instance.InstantiateInPool(
                      vfxExplosion.gameObject,
                      transform.position,
                      Quaternion.identity
                      );
            StartCoroutine(DisableAfterTime(0.2f));
        }
        private void Move()
        {
            GetComponent<Rigidbody>().velocity = new Vector3(moveInput.x, GetComponent<Rigidbody>().velocity.y, moveInput.z);

        }
        private void ShootInput()
        {
            if (CanReceiveInput() == false)
            {
                anim.SetBool("IsShooting", false);
                return;
            }
            if (robotIC.shoot && armAimRig.weight == 1)
            {
                
                anim.SetBool("IsShooting", true);
                // Só atira se já passou o tempo do fireRate
                if (Time.time >= lastShootTime + fireRate)
                {
                    bool result = this.OnAmmoUpdate?.Invoke(1) ?? false;
                    if (result)
                    {
                        anim.SetBool("IsShooting", true);
                        Shoot();
                        lastShootTime = Time.time; // Marca o tempo do último disparo
                    }
                }
            }
            else anim.SetBool("IsShooting", false);

        }

        private void HealInput()
        {
           if( robotIC.useLithiumBomb)
            {
                if (Time.time >= lastShootTime + 0.5f)
                {
                    bool result = this.OnLBUpdate?.Invoke() ?? false;
                    if (result)
                    {
                        anim.SetBool("IsHealing", true);
                        GameObject vfxInstance =ObjectPoolManager.instance.InstantiateInPool(
                        vfxHeal.gameObject,
                        transform.position,
                        Quaternion.identity
                        );
                        lastShootTime = Time.time; // Marca o tempo do último disparo
                        vfxInstance.transform.SetParent(this.transform);
                        battery.currentCharge = battery.currentCharge + 50f;
                    }
                }
            }
        }

     
        private void DropInput()
        {
            if (!canDrop) return; // Se está no cooldown, não faz nada

            int halfGears = ((int)(weight / 1.5f)) / 2;
            gearsAmount = halfGears;

            if (robotIC.dropUnitItens && this.OnDropGears.Invoke(1))
            {
                int Gears = (int)(weight / 1.5f);
                gearsAmount = 1;
                Gears -= (int)gearsAmount;
                DropGears();

                StartCoroutine(DropCooldown(0.7f)); // Cooldown de 0.7s para dropar
            }
            //else if (robotIC.dropHalfItens && this.OnDropGears.Invoke(1))
            //{
            //    DropGears();
            //}
        }

     


        private void handleWeight()
        {
            int gears = 0;
            if ((this.OnGetGears?.Invoke()) != null)
                gears = (int)this.OnGetGears?.Invoke();

            if(gears != 0)
            weight = gears * 1.5f;

            // Normaliza peso para um valor de 0 a 1 (0 = peso 0, 1 = peso máximo 70)
            float t = Mathf.InverseLerp(0f, 70f, weight);

            // Faz o Lerp da velocidade máxima até mínima baseado em t
            currentVelocity = Mathf.Lerp(maxVelocity, minVelocity, t);
            if(weight > 70)
            fsmManager.SetBool("IsOverWeight", true);
            else
            fsmManager.SetBool("IsOverWeight", false);
        }
        public void CamMove()
        {
            if (CanReceiveInput() == false)
                return;

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

            float weightSpeed = 2f;
            Transform armAimTransform = transform.Find("RobotRenderer/AnimationRiggings/ArmAim");
            Transform headAimTransform = transform.Find("RobotRenderer/AnimationRiggings/BodyAim");
            if (armAimTransform != null)
            {
                armAimRig = armAimTransform.GetComponent<UnityEngine.Animations.Rigging.Rig>();
                headAimRig = headAimTransform.GetComponent<UnityEngine.Animations.Rigging.Rig>();
            }
            // --- Rotação do corpo só quando atirando ---
            if (robotIC.shoot)
            {
                armAimRig.weight = Mathf.MoveTowards(
                    armAimRig.weight, // valor atual
                    1f,               // alvo
                    weightSpeed * Time.deltaTime // velocidade
                );

                headAimRig.weight = Mathf.MoveTowards(
                    armAimRig.weight, // valor atual
                    1f,               // alvo
                    weightSpeed * Time.deltaTime // velocidade
                );

                // --- Rotação do braço seguindo o mouse ---

                Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
                Vector3 aimPoint = ray.GetPoint(50f); // Ponto distante (50 unidades à frente)
                Vector3 aimDir = (aimPoint - spawnBulletPosition.transform.position).normalized;

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
                spawnBulletPosition.transform.rotation = targetArmRot;



                // Distância máxima que o braço pode estender
                float maxReach = 5f;

                // Direção e distância entre braço e ponto
                aimDir = aimPoint - transform.position; // transform = corpo do robô
                float dis = Mathf.Min(aimDir.magnitude, maxReach); // limita o alcance
                aimDir.Normalize();

                // Posição alvo da ponta do braço
                Vector3 targetArmPos = transform.position + aimDir * dis;

                // Move o braço até a posição alvo
                robotArm.transform.position = targetArmPos;

                transform.rotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(aimDir, Vector3.up),
                    Vector3.up
                );
            }
            else
            {
                armAimRig.weight = Mathf.MoveTowards(
                    armAimRig.weight, // valor atual
                    0f,               // alvo
                    weightSpeed * Time.deltaTime // velocidade
                );
                headAimRig.weight = Mathf.MoveTowards(
                    armAimRig.weight, // valor atual
                    0f,               // alvo
                    weightSpeed * Time.deltaTime // velocidade
                );
            }

        }
        public bool ChecKGroundStatus()
        {
            float radius = 0.4f;
            float distance = 0.7f;
            int ignoreLayer = 3; int layerMask = ~(1 << ignoreLayer);

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
            float radius = 0.4f;
            float distance = 0.7f;
            Vector3 pos = transform.position + Vector3.down * distance;

            // Desenha a esfera
            Gizmos.DrawWireSphere(pos, radius);
        }

        private IEnumerator DropCooldown(float time)
        {
            canDrop = false;
            yield return new WaitForSeconds(time);
            canDrop = true;
        }

        private IEnumerator DisableAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false); // Desativa o objeto
        }
    }
}
