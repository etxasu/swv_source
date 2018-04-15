//NB: 01/31/2018
//script to resize testObject to make it easier to click

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeTestObjectSource : MonoBehaviour {

	public GameObject testObject;
	public GameObject objectLabel;
	public float labelVertDif = 4.2f;
	public float testVertDif = 5.0f;
	public float testHorDif = 7.0f;

	//NB: 04/11/2018
	//Changed to only resize to larger for mobile
	// Use this for initialization
	void Start () {
		if (SystemInfo.operatingSystem.ToLower().Contains("ios") || SystemInfo.operatingSystem.ToLower().Contains("android")) {
			testObject = GameObject.Find("TestObjectSource");
			Rescale();
			Relocate();
			MoveLabel();
		}
	}
		
	//rescale the image to be easier to click
	void Rescale() {
		testObject.transform.localScale += new Vector3(0.4f, 0.4f, 0.4f);
	}

	//move the image to not overlap with Minimap edge
	void Relocate() {
		testObject.transform.position += Vector3.up * testVertDif;
		testObject.transform.position -= Vector3.right * testHorDif;
	}

	//moves the label to not overlap with Minimap edge
	void MoveLabel() {
		objectLabel = testObject.transform.GetChild(4).gameObject;
		objectLabel.transform.position += Vector3.up * labelVertDif;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
