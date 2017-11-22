using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFS : MonoBehaviour {
	bool enableFull = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown(){
		if (enableFull) {
			
			Screen.SetResolution (Screen.width, Screen.height, true);
			//gameObject.ge ().sprite = OffSprite;
			enableFull = false;
			Debug.Log("FullScreen ON");
		} 
		else {
			//Android
			if (Application.platform == RuntimePlatform.Android) {
				Application.Quit ();
				//gameObject.GetComponent<Image>().sprite = OnSprite;
				enableFull = true;
				Debug.Log ("FullScreen OFF");



			}
			//Other 
			else {
				Screen.SetResolution (Screen.width,Screen.height, false);
				//gameObject.GetComponent<Image>().sprite = OnSprite;
				enableFull = true;
				Debug.Log ("FullScreen OFF");


			}

		}
	}
}
