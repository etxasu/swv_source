using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectAnimController : MonoBehaviour {

	public GenericElementFlasher SSM_Flasher;
	public GameObject TestObjectSource;
	public GameObject Arrow;
	public bool activateAnimation = false;
	public float timer = 0f;
	public int numFlashes = 0;

	// Use this for initialization
	void Start () {
		TestObjectSource = this.transform.parent.gameObject;
		SSM_Flasher = GameObject.Find ("SSM Flasher").GetComponent<GenericElementFlasher>();
		Arrow = this.gameObject.transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.deltaTime;

		//Check to see if test object is visible
		if (TestObjectSource.activeSelf && activateAnimation == false && timer > 5f) {
			//Start animating
			GetComponent<Animator>().enabled = true;
			activateAnimation = true;
			Arrow.SetActive (true);
			SSM_Flasher.TriggerFlash = true;
			numFlashes++;
			timer = 0f;
		} else if (TestObjectSource.activeSelf && activateAnimation == true && timer > 3f && numFlashes <= 2) {
			//Flash every 3 seconds for a total of 3 times
			SSM_Flasher.TriggerFlash = true;
			numFlashes++;
			timer = 0f;
		}
		//Destroy the game object after the flasher has flashed 3 times
		if (numFlashes > 2) {
			//Destroy this gameObject
			Destroy (gameObject);
		}
	}
}