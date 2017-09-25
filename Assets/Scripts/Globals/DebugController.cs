using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class DebugController : MonoBehaviour
{
    public GameObject CropDemo;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(CrossPlatformInputManager.GetButtonUp("DebugToggle"))
        {
            Debug.Log("Toggling Crop Demonstration: " + CropDemo.GetComponent<RectTransform>().rect.width + "x" + CropDemo.GetComponent<RectTransform>().rect.height);
            if (CropDemo.activeSelf)
            {
                CropDemo.SetActive(false);
            }
            else
            {
                CropDemo.SetActive(true);
            }
        }

    }
}
