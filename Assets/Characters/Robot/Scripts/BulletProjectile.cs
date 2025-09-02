using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BGJ14
{
    public class BulletProjectile : MonoBehaviour
    {
        private Rigidbody bulletRb;
        private Collider  bulletOwner;
        [SerializeField] Transform vfx;
        private float power;

        private void Awake()
        {
            bulletRb = GetComponent<Rigidbody>();
        }
        private void OnEnable()
        {
            float speed = 50f;
            bulletRb.velocity = transform.forward * speed;
            StartCoroutine(DisableAfterTime(4f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.Equals(bulletOwner))
            {
               Instantiate(vfx, transform.position, Quaternion.identity);
    
               // Tenta pegar o componente Battery no objeto atingido
               Battery battery = other.GetComponent<Battery>();
       
               if (battery != null)
               {
                   battery.Drain(1f * power);
                    if(bulletOwner.tag == "Sentinel" && other.tag == "Enemy")
                    {
                        battery.isSentinelShooting = true;
                        Debug.Log("isSentinelShooting: " + battery.isSentinelShooting);
                    }
                    if (bulletOwner.tag == "Player" && other.tag == "Enemy")
                    {
                        battery.isSentinelShooting = false;
                        Debug.Log("isSentinelShooting: " + battery.isSentinelShooting);
                    }
                }
       
               gameObject.SetActive(false);
            }
        }

        public void SetPower(float power)
        {
            this.power = power;
        }
        public void SetCollider(Collider bulletOwner)
        {
            this.bulletOwner = bulletOwner;
        }
        private IEnumerator DisableAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false); // Desativa o objeto
        }
    }


}
