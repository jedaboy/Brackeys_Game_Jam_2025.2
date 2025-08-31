using GRD.FSM;
using UnityEngine;
using UnityEngine.Rendering;

namespace BGJ14
{

    public class CharacterController : MonoBehaviour
    {

        public Animator anim;
        public new Rigidbody rigidbody;
        public Collider collider;
        public Battery battery;
        public FSM_Manager fsmManager;
        public float gearsAmount = 6f;
        [SerializeField] private Transform bulletProjectile;
        [SerializeField] private Transform gearSpawn;
        [SerializeField] public Transform spawnBulletPosition;
        [SerializeField] float bulletPower;
        [SerializeField] private CharacterSoundManager soundManager;

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
            bullet.GetComponent<BulletProjectile>().SetCollider(collider);

            Collider bulletCol = bullet.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>(); // ou pegue os colliders do corpo todo
            soundManager.PlaySound(soundManager.shootSound);

        }

        public virtual void DropGears()
        {
            for (int i = 0; i < gearsAmount; i++)
            {
                Vector3 spawnPos;
                if (collider.CompareTag("Player"))
                {
                    // Posição inicial com pequena variação (pra não cair tudo empilhado)
                    spawnPos = transform.position + new Vector3(
                    Random.Range(0.5f, 1f),
                    0.5f, // um pouco acima
                    Random.Range(0.5f, 1f)
                );
                }else
                {
                    // Posição inicial com pequena variação (pra não cair tudo empilhado)
                    spawnPos = spawnBulletPosition.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0.5f, // um pouco acima
                    Random.Range(-0.5f, 0.5f)
                    );
                }

                Transform gear = ObjectPoolManager.instance.InstantiateInPool(
                    gearSpawn.gameObject,
                    spawnPos,
                    Quaternion.identity
                ).transform;
                // Informa quem é o dono do collider (pra evitar bug de colisão instantânea)
                if (collider.CompareTag("Player"))
                {
                    gear.GetComponent<Gears>().isLocked = true;
                    gear.GetComponent<Gears>().WaitToCollect(3f);
                }
                else
                    gear.GetComponent<Gears>().isLocked = false;

                // Dá o pulo inicial
                Rigidbody rb = gear.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
                    rb.AddForce(randomDir * 3f, ForceMode.Impulse);
                }

              

                gear.GetComponent<Gears>().SetCollider(collider);
            }
        }


    }
}