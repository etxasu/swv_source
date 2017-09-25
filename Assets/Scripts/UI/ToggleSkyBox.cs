using UnityEngine;
using System.Collections;

public class ToggleSkyBox : MonoBehaviour
{
    public GameObject MyTargetCamera;
    public Material AlternateSky;

    private Material OriginalSky;

	// Use this for initialization
	void Start ()
    {
        OriginalSky = RenderSettings.skybox;
        //Debug.Log(OriginalSky.name);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // This does exactly what you think it does.
    // JOS: 6/29/2016
    public void ToggleSky()
    {
        if(RenderSettings.skybox == AlternateSky)
        {
           RenderSettings.skybox = OriginalSky;
        }
        else
        {
            RenderSettings.skybox = AlternateSky;
        }

    }

    public void ExposeMyCapi()
    {
        
    }
}
