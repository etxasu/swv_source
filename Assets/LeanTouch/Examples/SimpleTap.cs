using UnityEngine;

// This script will spawn a prefab when you tap the screen
public class SimpleTap : MonoBehaviour
{
	public GameObject Prefab;
    public GameObject IgnoredObject;
    public Camera MyCamera;
	
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerTap event
		Lean.LeanTouch.OnFingerTap += OnFingerTap;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook into the OnFingerTap event
		Lean.LeanTouch.OnFingerTap -= OnFingerTap;
	}
	
	public void OnFingerTap(Lean.LeanFinger finger)
	{
        Debug.Log("DEM TAPS");
		// Does the prefab exist?
		if (Prefab != null)
		{
			// Make sure the finger isn't over any GUI elements
			if (finger.IsOverGui == false)
			{
				/*
                // Clone the prefab, and place it where the finger was tapped
				var position = finger.GetWorldPosition(10.0f);
				var rotation = Quaternion.identity;
				var clone    = (GameObject)Instantiate(Prefab, position, rotation);
				
				// Make sure the prefab gets destroyed after some time
				Destroy(clone, 2.0f);
                */
			}
            else
            {
                if(finger.WhatTouched() == IgnoredObject)
                {
                    var position = new Vector3(finger.ScreenPosition.x, 0.0f, finger.ScreenPosition.y);
                    var rotation = Quaternion.identity;
                    var clone = (GameObject)Instantiate(Prefab, position, rotation);
                    Destroy(clone, 2.0f);
                }
            }
		}
	}
}