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
        private float lastLitiusTime;

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
                if (Time.time >= lastLitiusTime + 0.5f)
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
                        lastLitiusTime = Time.time; // Marca o tempo do último disparo
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

                // Calcula a distância dinâmica baseada no ângulo de pitch
                float dynamicDistance = distance;
                
                // Quando mira para cima ou para baixo, reduz a distância da câmera
                // Quanto mais extremo o ângulo, mais perto a câmera fica
                float pitchFactor = Mathf.Abs(pitch) / maxY; // 0 a 1 baseado no ângulo
                float distanceReduction = 1.0f;
                
                // Ajusta a redução baseado na direção da mira
                if (Mathf.Abs(pitch) > 0f) // Só começa a reduzir após 20 graus
                {
                    distanceReduction = 1.0f - (pitchFactor * 0.6f); // Reduz até 60% da distância
                    dynamicDistance = distance * distanceReduction;
                }

                // cria uma rotação só de yaw (horizontal, usada para deslocamentos laterais e trás)
                Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

                // offset configurável
                float sideOffset = 1f;  // ombro: positivo = direita, negativo = esquerda
                float height = 1f;      // altura da câmera

                // calcula posição desejada
                Vector3 desiredPosition =
                    transform.position
                    + Vector3.up * height
                    + yawRotation * (Vector3.right * sideOffset)
                    + yawRotation * (Vector3.back * dynamicDistance);

                // --- Colisão com paredes ---
                RaycastHit hit;
                Vector3 finalPosition = desiredPosition;
                Vector3 cameraTarget = transform.position + Vector3.up * height;
                
                // Usa SphereCast para detecção de colisão mais suave
                if (Physics.SphereCast(cameraTarget, 0.3f, 
                    (desiredPosition - cameraTarget).normalized, 
                    out hit, dynamicDistance))
                {
                    finalPosition = hit.point + hit.normal * 0.2f;
                }
                
                m_Cam.transform.position = finalPosition;

                // --- Rotação da câmera (pitch + yaw) ---
                Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
                m_Cam.transform.rotation = rotation;

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
                    armAimRig.weight = Mathf.MoveTowards(armAimRig.weight, 1f, weightSpeed * Time.deltaTime);
                    headAimRig.weight = Mathf.MoveTowards(headAimRig.weight, 1f, weightSpeed * Time.deltaTime);

                    // Direção de mira = frente da câmera
                    Vector3 aimDir = m_Cam.transform.forward;

                    // Rotaciona a arma/spawn na direção da câmera
                    spawnBulletPosition.rotation = Quaternion.LookRotation(aimDir, Vector3.up);

                    // Move o braço acompanhando a mira
                    float reach = 5f;
                    robotArm.transform.position = spawnBulletPosition.position + aimDir * reach;
                }
                else
                {
                    armAimRig.weight = Mathf.MoveTowards(armAimRig.weight, 0f, weightSpeed * Time.deltaTime);
                    headAimRig.weight = Mathf.MoveTowards(headAimRig.weight, 0f, weightSpeed * Time.deltaTime);
                }

                // Gira o corpo do robô junto com a câmera (ignora pitch, só yaw)
                Quaternion bodyRotation = Quaternion.Euler(0f, yaw, 0f);
                transform.rotation = bodyRotation;
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
