using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Name: FullScreen.cs
 * Author: Krystle Jones
 * Purpose: Detects the device screen resolution and sets screen to full upon user request 
**/

public class FullScreen : MonoBehaviour {

	//Simple ENABLE FULLSCREEN MODE |ON|OFF|
	private Rect fullPanel = new Rect((Screen.width - 300), (Screen.height - 60), 300, 300);

	//User prompt to enable fullscreen in order to enhance game experience
	private Rect promptPanel = new Rect((Screen.width - 600)/2, (Screen.height - 50)/2, 600, 50);

	//Boolean used to display prompt
	private bool prompt = false;
	//Used to trigger full screen enabled.
	private bool enableFull = false;

	void OnGUI () 
	{
		if(prompt)
			promptPanel = GUI.Window (0, promptPanel, promptWindow, "Gameplay Experience Enhanced when played in Fullscreen Mode.");
		
						
		fullPanel = GUI.Window (0, fullPanel, panelWindow, "Fullscreen Mode: ");
	}
		
	// Panel Window
	void panelWindow (int windowID)
	{
		float y = 20;
	

		if(GUI.Button(new Rect(5,y, fullPanel.width/2,  20), "ON"))
		{
			//Set Fullscreen
			Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);
			enableFull = true;

		}

		if(GUI.Button(new Rect(5,y, fullPanel.width/2, 40), "OFF"))
		{
			//User selects no, make the prompt go away.
			enableFull = false;
		}
	}
	// Prompt Window
	void promptWindow (int windowID)
	{
		float y = 20;

		if(GUI.Button(new Rect(5,y, promptPanel.width - 10, 20), "OK"))
		{
			prompt = false;
		}

	}
		
	// Use this for initialization
	void Start () {
		prompt = true;
	}

	// Update is called once per frame
	void Update () {
		if(enableFull)
			Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);
	}

}

	

