using GRD.FSM;
using UnityEngine;
using UnityEngine.Rendering;

namespace BGJ14
{

    public class CharacterController : MonoBehaviour
    {

        public Animator anim;
        public new Rigidbody rigidbody;
        public Collider Collider;
        public Battery battery;
        public FSM_Manager fsmManager;
        public float gearsAmount = 6f;
        [SerializeField] private Transform bulletProjectile;
        [SerializeField] private Transform gearSpawn;
        [SerializeField] public Transform spawnBulletPosition;
        [SerializeField] private float bulletPower;

        public virtual void Setup(float? bulletPower = null)
        {
            battery.currentCharge = battery.maxCharge;
            if (bulletPower != null)
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
            bullet.GetComponent<BulletProjectile>().SetCollider(Collider);

            Collider bulletCol = bullet.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>(); // ou pegue os colliders do corpo todo

        }

        public virtual void DropGears()
        {
            for (int i = 0; i < gearsAmount; i++)
            {
                // Posição inicial com pequena variação (pra não cair tudo empilhado)
                Vector3 spawnPos = spawnBulletPosition.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0.5f, // um pouco acima
                    Random.Range(-0.5f, 0.5f)
                );

                Transform gear = ObjectPoolManager.instance.InstantiateInPool(
                    gearSpawn.gameObject,
                    spawnPos,
                    Quaternion.identity
                ).transform;

                // Dá o pulo inicial
                Rigidbody rb = gear.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
                    rb.AddForce(randomDir * 3f, ForceMode.Impulse);
                }

                // Informa quem é o dono do collider (pra evitar bug de colisão instantânea)
                gear.GetComponent<Gears>().SetCollider(Collider);
            }
        }


    }
}