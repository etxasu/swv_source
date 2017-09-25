using UnityEngine;
using System.Collections;

public class SolarSystemViewerButton : MonoBehaviour
{
    public GameObject TargetSSV;

    public Vector3 NewScale;
    private Vector3 OriginalScale;

    // Never assign a Z
    public Vector3 New2DPosition;
    private Vector3 Original2DPosition;

	// Use this for initialization
	void Start ()
    {
        OriginalScale = TargetSSV.transform.localScale;
        Original2DPosition = TargetSSV.transform.localPosition;
        Debug.Log(Original2DPosition);


    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ToggleSSV()
    {
        if(TargetSSV.transform.localScale == OriginalScale)
        {
            TargetSSV.transform.localScale = NewScale;
            TargetSSV.transform.localPosition = New2DPosition;
        }
        else
        {
            TargetSSV.transform.localScale = OriginalScale;
            TargetSSV.transform.localPosition = Original2DPosition;
        }
        

    }

}
