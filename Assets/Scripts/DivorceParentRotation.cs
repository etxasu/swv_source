using UnityEngine;
using System.Collections;

public class DivorceParentRotation : MonoBehaviour
{
    private Quaternion startRotation;
    public bool UseCustomInput;
    public Vector3 RotationSpeed;
	
    // Use this for initialization
	void Start ()
    {
        startRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void LateUpdate()
    {
        if (UseCustomInput)
        {
            transform.rotation = Quaternion.Euler(RotationSpeed);
        }
        else
        {
            transform.rotation = startRotation;
        }
        //transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

}
