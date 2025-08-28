using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGJ14
{
    public class BulletProjectile : MonoBehaviour
    {
        private Rigidbody bulletRb;
        [SerializeField] Transform vfx;
        private void Awake()
        {
            bulletRb = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            float speed = 25f;
            bulletRb.velocity = transform.forward * speed;
            Destroy(gameObject, 4f);
        }

        private void OnTriggerEnter(Collider other)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);

            if (other.CompareTag("Robot"))
            {
                // Tenta pegar o componente Battery no objeto atingido
                Battery battery = other.GetComponent<RobotController>()?.GetComponent<Battery>();

                if (battery != null)
                {
                    battery.Drain(1f);
                }
            }

            Destroy(gameObject);
        }

    }
}