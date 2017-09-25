using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class MinimapHoverNameplate : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(CrossPlatformInputManager.GetAxis("Mouse X") != 0.0f || CrossPlatformInputManager.GetAxis("Mouse Y") != 0.0f)
        {
            Vector2 _mPos = Input.mousePosition;
            // Debug.Log(_mPos.ToString());
            transform.position = new Vector3(_mPos.x + 80, _mPos.y, 0.0f);
        }	
	}
}
