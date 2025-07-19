using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoidFlockingComponent : MonoBehaviour
{
    public Transform target;
    public LayerMask flockLayer;

    [Header("Weights")]
    public float cohesionWeight = 1f;
    public float separationWeight = 1f;
    public float alignmentWeight = 1f;

    [Header("Radii")]
    public float separationRadius = 2f;
    public float cohesionRadius = 5f;
    public float alignmentRadius = 5f;

    [Header("Movement")]
    public float moveSpeed = 7f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 flockDirection = CalculateFlocking();
        if (flockDirection != Vector3.zero)
        {
            Vector3 velocity = flockDirection.normalized * moveSpeed;
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

            // Zero vertical rotation when turning
            Vector3 lookDir = flockDirection;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
            }
        }

        Debug.DrawRay(transform.position, flockDirection * 2f, Color.green);
    }

    Vector3 CalculateFlocking()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, Mathf.Max(separationRadius, cohesionRadius, alignmentRadius), flockLayer);

        int separationCount = 0;
        int alignmentCount = 0;
        int cohesionCount = 0;

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;

            float dist = Vector3.Distance(transform.position, neighbor.transform.position);

            if (dist < separationRadius)
            {
                Vector3 toNeighbor = transform.position - neighbor.transform.position;
                separation += toNeighbor.normalized / Mathf.Max(toNeighbor.magnitude, 0.01f);
                separationCount++;
            }

            if (dist < alignmentRadius)
            {
                alignment += neighbor.transform.forward;
                alignmentCount++;
            }

            if (dist < cohesionRadius)
            {
                cohesion += neighbor.transform.position;
                cohesionCount++;
            }
        }

        Vector3 separationVec = separationCount > 0 ? (separation / separationCount) * separationWeight : Vector3.zero;
        Vector3 alignmentVec = alignmentCount > 0 ? (alignment / alignmentCount).normalized * alignmentWeight : Vector3.zero;
        Vector3 cohesionVec = cohesionCount > 0 ? ((cohesion / cohesionCount) - transform.position).normalized * cohesionWeight : Vector3.zero;

        Vector3 toTarget = target ? (target.position - transform.position).normalized : Vector3.zero;

        return separationVec + alignmentVec + cohesionVec + toTarget;
    }
}
