using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// This script allows you to drag this GameObject using any finger, as long it has a collider
public class SimpleDrag : MonoBehaviour
{
    public Camera CameraToUse;

	// This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	
	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger draggingFinger;
	
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
	
	protected virtual void LateUpdate()
	{
		// If there is an active finger, move this GameObject based on it
		if (draggingFinger != null)
		{
            Vector2 _mPos = new Vector2(CrossPlatformInputManager.GetAxis("Mouse X"), CrossPlatformInputManager.GetAxis("Mouse Y"));
           // Debug.Log(_mPos.ToString());
			transform.parent.transform.position += new Vector3(_mPos.x, 0.0f, _mPos.y);
		}
	}

    public void OnFingerDown(Lean.LeanFinger finger)
	{
        //Debug.Log("OnFingerDown called by " + gameObject.name);
        // Raycast information
        Ray ray = finger.GetRay(CameraToUse);
		RaycastHit hit;
        //Debug.DrawRay(CameraToUse.transform.position, CameraToUse.transform.forward, Color.green, 10, true);

        //Debug.Log(ray.origin);
        //Debug.Log(transform.position + "HI");

        // Was this finger pressed down on a collider?

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask) == true)
		{
            //Debug.Log(hit.transform.name);
            // Was that collider this one?
            if (hit.collider.gameObject == gameObject)
			{
                //Debug.Log("Touched " + hit.collider.gameObject.name);
				// Set the current finger to this one
				draggingFinger = finger;
			}
		}
	}
	
	public void OnFingerUp(Lean.LeanFinger finger)
	{
		// Was the current finger lifted from the screen?
		if (finger == draggingFinger)
		{
			// Unset the current finger
			draggingFinger = null;
		}
	}
}