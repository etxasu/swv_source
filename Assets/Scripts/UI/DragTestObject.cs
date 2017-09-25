using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

// This script allows you to drag this GameObject using any finger, as long it has a collider
public class DragTestObject : MonoBehaviour, IDragHandler
{
    RectTransform _Transform = null;
    private Vector3 _StartPosition;

    protected virtual void OnEnable()
    {
        // Hook into the OnFingerDown event
        Lean.LeanTouch.OnFingerDown += OnFingerDown;

        // Hook into the OnFingerUp event
        Lean.LeanTouch.OnFingerUp += OnFingerUp;
    }

    protected virtual void OnDisable()
    {
        // Unhook the OnFingerDown event
        Lean.LeanTouch.OnFingerDown -= OnFingerDown;

        // Unhook the OnFingerUp event
        Lean.LeanTouch.OnFingerUp -= OnFingerUp;
    }

    // Use this for initialization
    void Start()
    {
        _Transform = GetComponent<RectTransform>();
        _StartPosition = _Transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _Transform.position += new Vector3(eventData.delta.x, eventData.delta.y);

        // magic : add zone clamping if's here.
    }

    public void OnFingerUp(Lean.LeanFinger finger)
    {

    }

    public void OnFingerDown(Lean.LeanFinger finger)
    {
        // de nada
    }
}