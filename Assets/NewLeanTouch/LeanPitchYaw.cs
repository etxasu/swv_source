using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;

namespace NewLean.Touch
{
	// This script allows you to tilt & pan the current GameObject (e.g. camera) by dragging your finger(s)
	//[ExecuteInEditMode]
    [RequireComponent(typeof(CharacterController))]
    public class LeanPitchYaw : MonoBehaviour
	{
        
        public GameObject TouchedElement;

        [Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("If you want the rotation to be scaled by the camera FOV, then set that here")]
		public Camera Camera;

        [Space(10.0f)]
        [Tooltip("Pitch of the rotation in degrees")]	
		public float Pitch;

		[Tooltip("The strength of the pitch changes with vertical finger movement")]
		public float PitchSensitivity = -0.05f;

		[Tooltip("Limit the pitch to min/max?")]
		public bool PitchClamp = true;

		[Tooltip("The minimum pitch angle in degrees")]
		public float PitchMin = -90.0f;

		[Tooltip("The maximum pitch angle in degrees")]
		public float PitchMax = 90.0f;

        [Space(10.0f)]
        [Tooltip("Yaw of the rotation in degrees")]	
		public float Yaw;

		[Tooltip("The strength of the yaw changes with horizontal finger movement")]
		public float YawSensitivity = -0.05f;

		[Tooltip("Limit the yaw to min/max?")]
		public bool YawClamp;

		[Tooltip("The minimum yaw angle in degrees")]
		public float YawMin = -45.0f;

		[Tooltip("The maximum yaw angle in degrees")]
		public float YawMax = 45.0f;


        public bool AutoTrackCurrentTarget;
        public bool ControlEnabled;
        public GameObject MySceneController;
        private bool firstUpdate = true;

#if UNITY_EDITOR
        protected virtual void Reset()
		{
			if (Camera == null)
			{
				Camera = GetComponent<Camera>();
			}
		}
#endif
        private void ExposeMyCapi()
        {
            Capi.expose<bool>("Camera.AutoTrackCurrentTarget", () => { return AutoTrackCurrentTarget; }, (value) => { return AutoTrackCurrentTarget = value; });
        }

        protected virtual void OnEnable()
        {
            // Hook into the OnFingerDown event
            NewLeanTouch.OnFingerDown += OnFingerDown;

            // Hook into the OnFingerUp event
            NewLeanTouch.OnFingerUp += OnFingerUp;
        }

        protected virtual void OnDisable()
        {
            // Unhook the OnFingerDown event
            NewLeanTouch.OnFingerDown -= OnFingerDown;

            // Unhook the OnFingerUp event
            NewLeanTouch.OnFingerUp -= OnFingerUp;
        }

        protected virtual void LateUpdate()
		{
			// Get the fingers we want to use
			var fingers = NewLeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);

			// Get the scaled average movement vector of these fingers
			var drag = NewLeanGesture.GetScaledDelta(fingers);

			// Get base sensitivity
			var sensitivity = GetSensitivity();

			// Adjust pitch
			Pitch += drag.y * PitchSensitivity * sensitivity;
          
			if (PitchClamp == true)
			{
				Pitch = Mathf.Clamp(Pitch, PitchMin, PitchMax);
			}

			// Adjust yaw
			Yaw -= drag.x * YawSensitivity * sensitivity;
           

            if (YawClamp == true)
			{
				Yaw = Mathf.Clamp(Yaw, YawMin, YawMax);
			}

            // Rotate to pitch and yaw values
            transform.localRotation = Quaternion.Euler(Pitch, 0.0f, 0.0f);
            transform.parent.localRotation = Quaternion.Euler(0.0f, Yaw, 0.0f);
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + Pitch, 0.0f, 0.0f);
            //transform.parent.localEulerAngles = new Vector3(0.0f, transform.parent.localEulerAngles.y + Yaw, 0.0f);

            if (firstUpdate)
            {
                ExposeMyCapi();
                firstUpdate = false;
            }
            if (!AutoTrackCurrentTarget)
            {
                if (CrossPlatformInputManager.GetAxis("Fire1") > 0.0f)
                {
                    if (ControlEnabled)
                    {
                        // Check to see if they're over a GUI element
                        if (TouchedElement != null && TouchedElement.layer == 12)
                        {
                            //RotateView();
                        }
                        else
                        {
                            RotateView();
                        }
                    }
                }
            }
            else
            {
                AutoTrackTarget();
            }

        }

        private void RotateView()
        {
            transform.LookAt(TouchedElement.transform);
        }

        public void OnFingerDown(NewLeanFinger finger)
        {
            Vector2 position = new Vector2(finger.ScreenPosition.x,finger.ScreenPosition.y);
            
            if (finger.WhatTouched(position) != null)
            {
                TouchedElement = finger.WhatTouched(position);
                AutoTrackCurrentTarget = true;
            }
            else
            {
                AutoTrackCurrentTarget = false;
                TouchedElement = null;
            }
            
        }

        public void OnFingerUp(NewLeanFinger finger)
        {
            if (ControlEnabled)
            {
                Capi.set("Camera.Rotation.x", this.transform.rotation.x);
                Capi.set("Camera.Rotation.y", this.transform.parent.rotation.y);
                Debug.Log("X:"+ transform.rotation.x + " Y:"+  transform.parent.rotation.y);      
            }
        }


        private float GetSensitivity()
		{
			// Has a camera been set?
			if (Camera != null)
			{
				// Adjust sensitivity by FOV?
				if (Camera.orthographic == false)
				{
					return Camera.fieldOfView / 90.0f;
				}
			}

			return 1.0f;
		}


        private void AutoTrackTarget()
        {
            transform.LookAt(MySceneController.GetComponent<SceneController>()._selected.transform);
            transform.parent.LookAt(MySceneController.GetComponent<SceneController>()._selected.transform);

            //Follow touched object
            if(TouchedElement != null)
                transform.LookAt(TouchedElement.transform);

            // Flatten our magic out
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, 0.0f);
            transform.parent.localEulerAngles = new Vector3(0.0f, transform.parent.localEulerAngles.y, 0.0f);
        }
    }
}