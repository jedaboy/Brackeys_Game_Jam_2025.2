using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BGJ14
{
    public class BulletProjectile : MonoBehaviour
    {
        private Rigidbody bulletRb;
        [SerializeField] Transform vfx;
        private float power;

        private void Awake()
        {
            bulletRb = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            float speed = 25f;
            bulletRb.velocity = transform.forward * speed;
            //Destroy(gameObject, 4f); //implementar timer para desativar 
        }

        private void OnTriggerEnter(Collider other)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        
            // Tenta pegar o componente Battery no objeto atingido
            Battery battery = other.GetComponent<Battery>();

            if (battery != null)
            {
                battery.Drain(1f);
            }

            gameObject.SetActive(false);
        }

        public void SetPower(float power)
        {
            this.power = power;
        }

    }
}