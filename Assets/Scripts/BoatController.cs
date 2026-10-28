using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Range(-1f, 1f)] public float throttleInput;
    [Range(-1f, 1f)] public float steeringInput;
    public float forwardAcceleration = 8f;
    public float reverseAcceleration = 4f;
    public float maxForwardSpeed = 22f;
    public float maxReverseSpeed = 6f;
    public float turnSpeed = 35f;
    public float lateralGrip = 2f;
    public float SpeedKnots => GetComponent<Rigidbody>().velocity.magnitude * 1.94384f;
    private Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.centerOfMass = new Vector3(0f, -0.35f, 0f);
    }

    void FixedUpdate()
    {
        float accel = throttleInput >= 0f ? throttleInput * forwardAcceleration : throttleInput * reverseAcceleration;
        body.AddForce(transform.forward * accel, ForceMode.Acceleration);

        float forwardSpeed = Vector3.Dot(body.velocity, transform.forward);
        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / 2f);
        float reverseSteering = forwardSpeed < -0.2f ? -0.55f : 1f;
        body.MoveRotation(body.rotation * Quaternion.Euler(0f, steeringInput * turnSpeed * speedFactor * reverseSteering * Time.fixedDeltaTime, 0f));

        Vector3 local = transform.InverseTransformDirection(body.velocity);
        local.x = Mathf.MoveTowards(local.x, 0f, lateralGrip * Time.fixedDeltaTime);
        body.velocity = transform.TransformDirection(local);

        float along = Vector3.Dot(body.velocity, transform.forward);
        if (along > maxForwardSpeed) body.velocity -= transform.forward * (along - maxForwardSpeed);
        if (along < -maxReverseSpeed) body.velocity -= transform.forward * (along + maxReverseSpeed);
    }
}