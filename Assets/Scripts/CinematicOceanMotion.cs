using UnityEngine;

// Decorative motion only; this is not a wave simulation or boat collision surface.
public class CinematicOceanMotion : MonoBehaviour
{
    public float bobHeight = 0.08f, bobSpeed = 0.35f, slowYawDegrees = 0.25f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    void Start() { startPosition = transform.position; startRotation = transform.rotation; }
    void Update()
    {
        float t = Time.time * bobSpeed;
        transform.position = startPosition + Vector3.up * (Mathf.Sin(t) * bobHeight);
        transform.rotation = startRotation * Quaternion.Euler(0f, Mathf.Sin(t * 0.7f) * slowYawDegrees, 0f);
    }
}