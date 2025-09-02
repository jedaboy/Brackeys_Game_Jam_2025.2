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
        // campos da classe
        [SerializeField] private float aimBlendSpeed = 0.05f; // quanto maior, mais rápido transita (tweak)
        private float aimBlend = 0f; // 0 = livre, 1 = atirando

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
            //

            int gears = 0;
            if ((this.OnGetGears?.Invoke()) != null)
                gears = (int)this.OnGetGears?.Invoke();

            if (gears != 0)
                weight = gears * 1.5f;

            // Normaliza peso para um valor de 0 a 1 (0 = peso 0, 1 = peso máximo 70)
            float t = Mathf.InverseLerp(0f, battery.maxCharge, weight);

            // Faz o Lerp da velocidade máxima até mínima baseado em t
            currentVelocity = Mathf.Lerp(maxVelocity, minVelocity, t);
            if (weight > (battery.maxCharge))
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

            // --- Calcula alvo do modo LIVRE ---
            Vector3 freeDesiredPosition = transform.position + Vector3.up * 0.5f + rotation * new Vector3(0f, 2f, -distance);
            Vector3 freeFinalPosition = freeDesiredPosition;
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.up * 0.5f, freeDesiredPosition, out hit))
            {
                freeFinalPosition = hit.point + hit.normal * 0.2f;
            }

            // --- Calcula alvo do modo ATIRANDO ---
            // distância dinâmica baseada no pitch (mesma lógica sua)
            float dynamicDistance = distance;
            float pitchFactor = Mathf.Abs(pitch) / maxY;
            float distanceReduction = 1.0f;
            if (Mathf.Abs(pitch) > 0f)
            {
                distanceReduction = 1.0f - (pitchFactor * 0.6f);
                dynamicDistance = distance * distanceReduction;
            }

            Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
            float sideOffset = 1f;
            float height = 1f;

            Vector3 shootDesiredPosition =
                transform.position
                + Vector3.up * height
                + yawRotation * (Vector3.right * sideOffset)
                + yawRotation * (Vector3.back * dynamicDistance);

            Vector3 shootFinalPosition = shootDesiredPosition;
            Vector3 cameraTarget = transform.position + Vector3.up * height;
            float castDist = (shootDesiredPosition - cameraTarget).magnitude;
            if (castDist > 0.001f && Physics.SphereCast(cameraTarget, 0.3f, (shootDesiredPosition - cameraTarget).normalized, out hit, castDist))
            {
                shootFinalPosition = hit.point + hit.normal * 0.2f;
            }

            // --- Atualiza blend entre estados (0 = livre, 1 = atirando) ---
            float targetBlend = robotIC.shoot ? 1f : 0f;
            aimBlend = Mathf.MoveTowards(aimBlend, targetBlend, (aimBlendSpeed * Time.deltaTime)/7f);

            // --- Posição final interpolada entre os dois modos ---
            Vector3 finalPosition = Vector3.Lerp(freeFinalPosition, shootFinalPosition, aimBlend);
            m_Cam.transform.position = finalPosition;

            // --- Rotação alvo de cada modo ---
            Quaternion freeRotation = Quaternion.LookRotation((transform.position + Vector3.up * 0.5f) - freeFinalPosition, Vector3.up);
            Quaternion shootRotation = Quaternion.Euler(pitch, yaw, 0f);

            // blendedRotation faz a transição entre os dois (quando aimBlend==0 => freeRotation sem suavização)
            Quaternion blendedRotation = Quaternion.Slerp(freeRotation, shootRotation, aimBlend);
            m_Cam.transform.rotation = blendedRotation;

            // --- Rigs (mantive sua suavização original) ---
            float weightSpeed = 2f;
            Transform armAimTransform = transform.Find("RobotRenderer/AnimationRiggings/ArmAim");
            Transform headAimTransform = transform.Find("RobotRenderer/AnimationRiggings/BodyAim");

            if (armAimTransform != null)
                armAimRig = armAimTransform.GetComponent<UnityEngine.Animations.Rigging.Rig>();
            if (headAimTransform != null)
                headAimRig = headAimTransform.GetComponent<UnityEngine.Animations.Rigging.Rig>();

            if (armAimRig != null)
                armAimRig.weight = Mathf.MoveTowards(armAimRig.weight, robotIC.shoot ? 1f : 0f, weightSpeed * Time.deltaTime);
            if (headAimRig != null)
                headAimRig.weight = Mathf.MoveTowards(headAimRig.weight, robotIC.shoot ? 1f : 0f, weightSpeed * Time.deltaTime);

            // --- Comportamento extra do modo atirando (mantive seu código) ---
            if (robotIC.shoot)
            {
                // Direção de mira = frente da câmera (já atualizada)
                Vector3 aimDir = m_Cam.transform.forward;

                // Rotaciona a arma/spawn na direção da câmera
                spawnBulletPosition.rotation = Quaternion.LookRotation(aimDir, Vector3.up);

                // Move o braço acompanhando a mira
                float reach = 5f;
                robotArm.transform.position = spawnBulletPosition.position + aimDir * reach;

                // Gira o corpo do robô junto com a câmera (ignora pitch, só yaw)
                Quaternion bodyRotation = Quaternion.Euler(0f, yaw, 0f);
                transform.rotation = bodyRotation;
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
