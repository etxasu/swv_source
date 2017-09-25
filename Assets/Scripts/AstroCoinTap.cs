using UnityEngine;

// This script will spawn a prefab when you tap the screen
public class AstroCoinTap : MonoBehaviour
{
	public GameObject Prefab;
    public GameObject IgnoredObject;
    public Camera MyCamera;

    private Lean.LeanFinger MyFinger;
	
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerTap event
		//Lean.LeanTouch.OnFingerTap += OnFingerTap;

        // Hook into the OnFingerDown event
        Lean.LeanTouch.OnFingerDown += OnFingerDown;

        // Hook into the OnFingerUp event
        //Lean.LeanTouch.OnFingerUp += OnFingerUp;
    }
	
	protected virtual void OnDisable()
	{
		// Unhook into the OnFingerTap event
		//Lean.LeanTouch.OnFingerTap -= OnFingerTap;

        // Unhook the OnFingerDown event
        Lean.LeanTouch.OnFingerDown -= OnFingerDown;

        // Unhook the OnFingerUp event
        //Lean.LeanTouch.OnFingerUp -= OnFingerUp;
    }
	
	public void OnFingerDown(Lean.LeanFinger finger)
	{
        // Does the prefab exist?
        Ray ray = finger.GetRay(MyCamera);
        RaycastHit hit;

        //Debug.DrawLine(MyCamera.transform.position, transform.position);
        Debug.DrawRay(MyCamera.transform.position, MyCamera.transform.forward);

        // Was this finger pressed down on a collider?
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 13) == true)
        {
            Debug.Log("I hit a " + hit.collider.name);
            // Was that collider this one?
            if ((hit.collider.gameObject.transform.parent == gameObject.transform.parent) && (hit.collider.gameObject.layer == 13))
            {
                // Set the current finger to this one
                MyFinger = finger;
                Debug.Log("DEM TAPS");
            }
        }
    }
}