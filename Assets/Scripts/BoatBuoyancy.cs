using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatBuoyancy : MonoBehaviour
{
    [Tooltip("Assign at least four transforms around the hull near the waterline.")]
    public Transform[] buoyancyPoints;
    public float waterLevel = 0f;
    public float buoyancyStrength = 18f;
    public float verticalDamping = 3.5f;
    private Rigidbody body;

    void Awake() { body = GetComponent<Rigidbody>(); }

    void FixedUpdate()
    {
        if (buoyancyPoints == null) return;
        foreach (Transform point in buoyancyPoints)
        {
            if (!point) continue;
            float depth = waterLevel - point.position.y;
            if (depth <= 0f) continue;
            float verticalSpeed = Vector3.Dot(body.GetPointVelocity(point.position), Vector3.up);
            float force = Mathf.Max(0f, depth * buoyancyStrength - verticalSpeed * verticalDamping);
            body.AddForceAtPosition(Vector3.up * force, point.position, ForceMode.Acceleration);
        }
    }
}