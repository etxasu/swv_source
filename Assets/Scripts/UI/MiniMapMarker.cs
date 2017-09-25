using UnityEngine;
using System.Collections;


//
// MiniMapMarker.cs
// 
// This class is intended to be attached to minimap marker objects in the inspector.
//
public class MiniMapMarker : MonoBehaviour
{
    private Camera MyCamera;
    private Vector3 RelativePosition;

    // Use this for initialization
    void Start ()
    {
        MyCamera = Camera.main;

	}
	
	// Update is called once per frame
	void Update ()
    {
        AdjustColor();
       
    }

    public void ToggleRendering(bool RenderMe)
    {
        gameObject.GetComponent<Renderer>().enabled = RenderMe;
    }

    // Checks the height of the minimap marker relative to the main camera.
    private void CheckHeight()
    {
        RelativePosition = MyCamera.transform.InverseTransformPoint(transform.position);
    }

    // Adjusts the marker color based on the y-difference between marker and camera.
    private void AdjustColor()
    {
        CheckHeight();

        if(RelativePosition.y > 0)
        {
            gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.cyan;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.red;
        }

    }
}
