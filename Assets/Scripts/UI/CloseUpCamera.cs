using UnityEngine;
using UnityEngine.UI;
using Lean;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CloseUpCamera : MonoBehaviour
{
    private Lean.LeanFinger Fingers;

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float ZoomIncrement = 5.0f;
    public float ZoomPrecision = 10.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;
    public float percentage = 0.5f;

    private Rigidbody myRigidbody;

    float x = 0.0f;
    float y = 0.0f;

    public Slider MyZoomSlider;
   
    #region Fingers Functions
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

    public void OnFingerDown(Lean.LeanFinger finger)
    {
        Fingers = finger;
        //Debug.Log("I HAS A FINGER");
    }

    public void OnFingerUp(Lean.LeanFinger finger)
    {
       // Debug.Log("I HAS A FINGER");
        if (Fingers == finger)
        {
            Fingers = null;
        }
    }

    #endregion

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        myRigidbody = GetComponent<Rigidbody>();

        SetMinAndMaxZoom(distanceMax, distanceMin, percentage);

        // Make the rigid body not change rotation
        if (myRigidbody != null)
        {
            myRigidbody.freezeRotation = true;
        }
    }

    public void SetMinAndMaxZoom(float _Max, float _Min, float _percentage)
    {
        distanceMin = _Min;
        distanceMax = _Max;

        MyZoomSlider.maxValue = _Max;
        MyZoomSlider.minValue = _Min;

        percentage = _percentage;

        ZoomIncrement = _Max / ZoomPrecision;
        ResetDistance(_percentage);
    }

    public void ResetZoomLevels()
    {
        SetMinAndMaxZoom(distanceMax, distanceMin, percentage);
    }

    void LateUpdate()
    {
        //Debug.Log(Fingers.ToString());
        
        if (target && Fingers != null)
        {
            if (Fingers.WhatTouched() == null)
            {
                x += Lean.LeanTouch.DragDelta.x * xSpeed * 0.02f;
                y -= Lean.LeanTouch.DragDelta.y * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                // This exists here because it used to say "MouseWheel" and I wanted to preserve the structure.
                // JOS: 10/20/2016
                distance = Mathf.Clamp(distance, distanceMin, distanceMax);
                float dst = distance;
                /*
                RaycastHit hit;
                if (Physics.Linecast(target.position, rotation * new Vector3(0f, 0f, -distance) + target.position, out hit))
                {
                    dst -= hit.distance;
                }
                */
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -dst);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;

                //Debug.Log("NO TOUCHY THE THING");
            }
            else // User is moving the slider or clicking on something else
            {
                KeepCameraOnWorld();
            }
        }
        else if(target) // Separate so angle is not updated between CUP transitions
        {
            KeepCameraOnWorld();
        }

        MyZoomSlider.value = distance;
        transform.LookAt(target);
    }

    private void KeepCameraOnWorld()
    {
        // This exists here because it used to say "MouseWheel" and I wanted to preserve the structure.
        // JOS: 10/20/2016
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
        float dst = distance;

        Quaternion rotation = transform.rotation;
        /*
        RaycastHit hit;
        if (Physics.Linecast(target.position, rotation * new Vector3(0f, 0f, -distance) + target.position, out hit))
        {
            dst -= hit.distance;
        }
        */
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -dst);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;

        //Debug.Log("MOUSING THE WHEEL");
    }

    public void ResetDistance(float _percentage)
    {
        //Debug.Log("RESETTING DISTANCE");
        float range =  percentage * (distanceMax-distanceMin);
		distance = distanceMin + range;
        //Debug.Log(distance);
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);

        //Debug.Log ("HI DUDE: " + distance);

        Quaternion rotation = transform.rotation;
        /*
        RaycastHit hit;
        if (Physics.Linecast(target.position, rotation * new Vector3(0f, 0f, -distance) + target.position, out hit))
        {
            dst -= hit.distance;
        }
        */
		Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

		//Debug.Log (position);

        transform.rotation = rotation;
        transform.position = position;
    }

    public void UpdateZoomBySlider()
    {
        distance = MyZoomSlider.value;
        Quaternion rotation = transform.rotation;
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;

        // check dat target
        transform.LookAt(target);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
