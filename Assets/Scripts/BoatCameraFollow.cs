using UnityEngine;

public class BoatCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5.2f, -10f);
    public float followSmoothness = 3.5f;
    public float lookHeight = 1.2f;
    public float speedFovBoost = 3f;
    private Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desired, followSmoothness * Time.deltaTime);
        Vector3 lookAt = target.position + Vector3.up * lookHeight + target.forward * 3f;
        Quaternion rotation = Quaternion.LookRotation(lookAt - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, followSmoothness * Time.deltaTime);
        BoatController boat = target.GetComponent<BoatController>();
        if (cam && boat)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f + Mathf.Clamp(boat.SpeedKnots / 15f, 0f, speedFovBoost), followSmoothness * Time.deltaTime);
    }
}