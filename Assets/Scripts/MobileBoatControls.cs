using UnityEngine;

public class MobileBoatControls : MonoBehaviour
{
    public BoatController boat;
    private bool throttle, reverse, left, right;

    void Update()
    {
        if (!boat) return;
        float vertical = (throttle ? 1f : 0f) - (reverse ? 1f : 0f);
        float horizontal = (right ? 1f : 0f) - (left ? 1f : 0f);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        boat.throttleInput = vertical;
        boat.steeringInput = horizontal;
    }

    public void HoldThrottle() { throttle = true; }
    public void ReleaseThrottle() { throttle = false; }
    public void HoldReverse() { reverse = true; }
    public void ReleaseReverse() { reverse = false; }
    public void HoldLeft() { left = true; }
    public void ReleaseLeft() { left = false; }
    public void HoldRight() { right = true; }
    public void ReleaseRight() { right = false; }
}