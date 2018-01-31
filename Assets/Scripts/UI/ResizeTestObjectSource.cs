//NB: 01/31/2018
//script to resize testObject to make it easier to click

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeTestObjectSource : MonoBehaviour {

	public GameObject testObject;
	public GameObject objectLabel;

	// Use this for initialization
	void Start () {
		testObject = GameObject.Find("TestObjectSource");
		rescale();
		relocate();
		moveLabel();
	}
		
	//rescale the image to be easier to click
	void rescale() {
		testObject.transform.localScale += new Vector3(0.4f, 0.4f, 0.4f);
	}

	//move the image to not overlap with Minimap edge
	void relocate() {
		testObject.transform.position += Vector3.up * 5f;
		testObject.transform.position -= Vector3.right * 7f;
	}

	//moves the label to not overlap with Minimap edge
	void moveLabel() {
		objectLabel = testObject.transform.GetChild(4).gameObject;
		objectLabel.transform.position += Vector3.up * 4.2f;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
