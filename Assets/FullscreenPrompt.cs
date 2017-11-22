using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenPrompt : MonoBehaviour {
	public Button promptButton;
	public Text promptText;
	public Image promptImage;


	// Use this for initialization
	void Start () {
		Button btn = promptButton.GetComponent<Button> ();
		btn.onClick.AddListener (DestroyOnClick);
	}

	// Update is called once per frame
	void Update () {
		
	}

	void DestroyOnClick(){
		Destroy (promptButton);
		Destroy (promptText);
		Destroy (promptImage);
	}

}
