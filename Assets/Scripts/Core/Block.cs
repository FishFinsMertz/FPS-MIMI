using Unity.Netcode;
using UnityEngine;

public class Block : NetworkBehaviour, IShootable
{
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Shoot(Vector3 pos, Vector3 vel, float m) 
    {
        rb.linearVelocity += vel * m / rb.mass;
        Vector3 r = pos - rb.worldCenterOfMass;

        Vector3 angularImpulse = Vector3.Cross(r, vel * m);
        Vector3 angularVelocityChange = new Vector3(
            angularImpulse.x / rb.inertiaTensor.x,
            angularImpulse.y / rb.inertiaTensor.y,
            angularImpulse.z / rb.inertiaTensor.z
        );
        rb.angularVelocity += rb.inertiaTensorRotation * angularVelocityChange;
    }
}
