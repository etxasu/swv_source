using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleMiniMapTarget : MonoBehaviour
{
    private Camera MainCamera;

    public float ZoomInFoV;
    private float OriginalFoV;

    private bool Zoomed;

    public RawImage MyRenderTarget;
    public RenderTexture MyRenderTexture;

	// Use this for initialization
	void Start ()
    {
        MainCamera = Camera.main;
        OriginalFoV = MainCamera.fieldOfView;
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // Swaps the target render texture to the inspector defined texture.
    public void Toggle()
    {
        MyRenderTarget.texture = MyRenderTexture;

    }

    // Swaps the main camera for the zoom camera and vice versa.
    public void SuperZoom()
    {
        //MainCamera.enabled = !MainCamera.enabled;

        Debug.Log(Camera.main.fieldOfView.ToString());
        if(Zoomed)
        {
            Camera.main.fieldOfView = OriginalFoV;
        }
        else
        {
            Camera.main.fieldOfView = ZoomInFoV;
        }
        Debug.Log(Camera.main.fieldOfView.ToString());
        Zoomed = !Zoomed;
    }
}
