using GRD.FSM;
using UnityEngine;

namespace BGJ14
{
    
    public class CharacterController : MonoBehaviour
    {

        public Animator anim;
        public new Rigidbody rigidbody;
        public Collider Collider;       
        public Battery battery;
        public FSM_Manager fsmManager;
        [SerializeField] private Transform bulletProjectile;
        [SerializeField] private Transform spawnBulletPosition;
        [SerializeField] float bulletPower;

        public virtual void Setup (float? bulletPower = null)
        {
            battery.currentCharge = battery.maxCharge;
            if(bulletPower != null)
            {
                this.bulletPower = bulletPower.Value;
            }
        }

        public virtual void Shoot()
        {
            Vector3 aimDir = spawnBulletPosition.forward;

            Transform bullet = ObjectPoolManager.instance.InstantiateInPool(
                bulletProjectile.gameObject,
                spawnBulletPosition.position,
                Quaternion.LookRotation(aimDir, Vector3.up)
            ).transform;
            bullet.GetComponent<BulletProjectile>().SetPower(bulletPower);

            Collider bulletCol = bullet.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>(); // ou pegue os colliders do corpo todo
       
        }

    }
}