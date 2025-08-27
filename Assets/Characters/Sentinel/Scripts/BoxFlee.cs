using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFlee : MonoBehaviour
{
    public float fleeSpeed = 5f;             // velocidade de fuga
    public float detectionRadius = 5f;       // raio de detecção do Sentinel
    public string sentinelTag = "Sentinel";  // tag do Sentinel
    public float randomness = 1f;            // intensidade da variação lateral
    public float fleeDistance = 10f;         // distância fixa do ponto de fuga
    public float safeDistance = 12f;         // distância mínima para considerar seguro

    private Rigidbody rb;
    private float fixedY;
    private Vector3 fleeTarget;              // ponto de fuga
    private bool isFleeing = false;
    private Vector3 randomOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fixedY = transform.position.y;

        randomOffset = Random.insideUnitSphere;
        randomOffset.y = 0;
        randomOffset.Normalize();
    }

    void FixedUpdate()
    {
        Transform closestSentinel = null;
        float minDist = Mathf.Infinity;

        // Detecta Sentinel mais próximo dentro do raio
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(sentinelTag))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestSentinel = hit.transform;
                }
            }
        }

        // Inicia fuga se detectar Sentinel
        if (closestSentinel != null && !isFleeing)
        {
            isFleeing = true;
            Vector3 dir = (transform.position - closestSentinel.position).normalized;
            fleeTarget = transform.position + dir * fleeDistance;
        }

        // Executa fuga
        if (isFleeing)
        {
            Vector3 direction = (fleeTarget - transform.position).normalized;

            // adiciona desvio lateral aleatório
            direction += randomOffset * randomness;
            direction.y = 0;
            direction.Normalize();

            Vector3 newPos = rb.position + direction * fleeSpeed * Time.fixedDeltaTime;
            newPos.y = fixedY;
            rb.MovePosition(newPos);

            // Atualiza direção lateral de vez em quando
            if (Random.value < 0.01f)
            {
                randomOffset = Random.insideUnitSphere;
                randomOffset.y = 0;
                randomOffset.Normalize();
            }

            // Verifica se está seguro de todos os Sentinels próximos
            bool safe = true;
            Collider[] allHits = Physics.OverlapSphere(transform.position, safeDistance);
            foreach (Collider hit in allHits)
            {
                if (hit.CompareTag(sentinelTag))
                {
                    safe = false;
                    break;
                }
            }

            // Para de fugir se estiver seguro
            if (safe)
            {
                isFleeing = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        if (isFleeing)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(fleeTarget, 0.3f);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
