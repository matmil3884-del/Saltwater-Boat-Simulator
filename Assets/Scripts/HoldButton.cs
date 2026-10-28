using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public MobileBoatControls controls;
    public HoldAction action;
    public enum HoldAction { Throttle, Reverse, Left, Right }
    public void OnPointerDown(PointerEventData eventData) { SetHeld(true); }
    public void OnPointerUp(PointerEventData eventData) { SetHeld(false); }
    public void OnPointerExit(PointerEventData eventData) { SetHeld(false); }

    private void SetHeld(bool held)
    {
        if (!controls) return;
        switch (action)
        {
            case HoldAction.Throttle: if (held) controls.HoldThrottle(); else controls.ReleaseThrottle(); break;
            case HoldAction.Reverse: if (held) controls.HoldReverse(); else controls.ReleaseReverse(); break;
            case HoldAction.Left: if (held) controls.HoldLeft(); else controls.ReleaseLeft(); break;
            case HoldAction.Right: if (held) controls.HoldRight(); else controls.ReleaseRight(); break;
        }
    }
    void OnDisable() { SetHeld(false); }
}