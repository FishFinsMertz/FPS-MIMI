using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // ONLY FOR GROUNDED ENEMIES
public class BoidFlockingComponent : NetworkBehaviour
{
    public Transform target;
    public LayerMask flockLayer;

    [Header("Weights")]
    public float cohesionWeight;
    public float separationWeight;
    public float alignmentWeight;

    [Header("Radii")]
    public float separationRadius;
    public float cohesionRadius;
    public float alignmentRadius;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Ground Checking")]
    public float groundRayDistance;
    public LayerMask groundLayer;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!IsServer) { return; }
        Vector3 flockDirection = CalculateFlocking();
        bool isGrounded = Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, groundRayDistance, groundLayer);

        if (flockDirection != Vector3.zero)
        {
            Vector3 movement = flockDirection.normalized;

            // Adjust movement to follow ground slope if grounded
            if (isGrounded)
            {
                movement = Vector3.ProjectOnPlane(movement, hit.normal).normalized;
                rb.linearVelocity = movement * moveSpeed;
            }
            else
            {
                // If not grounded, apply gravity or a falling effect
                rb.linearVelocity += Vector3.down * 20f * Time.fixedDeltaTime;
            }

            // rotate to face movement direction 
            if (movement != Vector3.zero)
            {
                Vector3 faceDirection = new Vector3(movement.x, movement.y, movement.z);
                if (faceDirection != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(faceDirection), Time.deltaTime * 10f);
            }
        }

        Debug.DrawRay(transform.position, flockDirection * 2f, Color.green);
    }

    private Vector3 CalculateFlocking()
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
