using GRD.FSM;
using UnityEngine;

namespace BGJ14
{
    public class RobotController : CharacterController
    {
        public PlayerInputController robotIC;
        public float weight;
        public float velocity;
        public bool gearsAffectWeight;
        public FSM_Manager fsmManager;
        public int ammo;
        public Camera m_Cam;
        public Transform alvo;
        private Vector3 moveInput;

        [SerializeField] private float distance = 5f;
        [SerializeField] private float sensitivity = 3f;
        [SerializeField] private float minY = 10f;
        [SerializeField] private float maxY = 60f;


        private float yaw;  
        private float pitch; 


        public void Update()
        {
            CamMove();
        }

        public void FixedUpdate()
        {
            Move();
        }

        public void MoveInput()
        {
            moveInput = robotIC.move.x * m_Cam.transform.right + robotIC.move.y * Vector3.ProjectOnPlane(m_Cam.transform.forward, Vector3.up).normalized;
        }

        public void JumpInput()
        {
            if (robotIC.jump)
            {
                Debug.Log("robotIC.Jump True:");
                fsmManager.SetBool("Jump", true);
            }
        }

        private void Move()
        {           
            rigidbody.velocity = new Vector3(moveInput.x, rigidbody.velocity.y, moveInput.z);         
        }
        public void CamMove()
        {

            Debug.Log("robotIC.camMove: " + robotIC.camMove);

            yaw += robotIC.camMove.x * sensitivity;
            pitch -= robotIC.camMove.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minY, maxY);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desiredPosition = transform.position + Vector3.up * 0.5f + rotation * new Vector3(0f, 0f, -distance);

            Vector3 finalPosition = desiredPosition;

            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.up * 0.5f, desiredPosition, out hit))
            {
                finalPosition = hit.point + hit.normal * 0.2f; // empurra um pouco pra fora da parede
            }

            m_Cam.transform.position = finalPosition;

            m_Cam.transform.rotation = Quaternion.LookRotation(
                (transform.position + Vector3.up * 0.5f) - m_Cam.transform.position,
                Vector3.up
            );


        }
    }
}