using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace BGJ14
{
    public class Gears : MonoBehaviour
    {
        private Rigidbody gearRb;
        private Collider gearOwner;
        [SerializeField] private int gearValue = 1;
        private bool collected = false;
        public bool isLocked;
        // Configurações do "hover"
        private float hoverHeight = 1f;
        private float floatAmplitude = 0.1f;
        private float floatFrequency = 2f;
        private Vector3 startPos;

        private void Awake()
        {
            gearRb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            collected = false;
            startPos = transform.position;

            if (gearRb != null)
            {
                gearRb.isKinematic = false; // deixa cair no chão quando nasce
                gearRb.useGravity = true;
            }

        }

        private void Update()
        {
            if (collected) return;

            // Mantém hover no chão
            if (transform.position.y < hoverHeight)
            {
                Vector3 pos = transform.position;
                pos.y = hoverHeight;
                transform.position = pos;

                if (gearRb != null)
                {
                    gearRb.isKinematic = true; // trava depois que encosta no chão
                    gearRb.useGravity = false;
                }
            }

            // Flutua e gira
            if (gearRb != null && gearRb.isKinematic)
            {
                float newY = hoverHeight + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);

                transform.Rotate(Vector3.up, 180f * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (collected) return;

            if (other.CompareTag("Player"))
            {


                if (!isLocked)
                {
                    RobotController robot = other.GetComponent<RobotController>();
                    Debug.Log("gearValue:  " + gearValue);
                    robot.OnCollectGear?.Invoke(gearValue);
                    collected = true;
                    gameObject.SetActive(false);
                }
            }
        }
        public void WaitToCollect(float time)
        {
            StartCoroutine(WaitingToCollect(time));
        }

        private IEnumerator WaitingToCollect(float time)
        {
            yield return new WaitForSeconds(time);
            isLocked = false;
            Debug.Log("destrancando isLocked:  " + isLocked);
        }
        public void SetCollider(Collider gearOwner)
        {
            this.gearOwner = gearOwner;
        }

    }
}
