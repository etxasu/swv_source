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
	private Rect fullPanel = new Rect((Screen.width - 300), (Screen.height - 50)/10, 300, 80);

	//User prompt to enable fullscreen in order to enhance game experience
	private Rect promptPanel = new Rect((Screen.width - 300)/2, (Screen.height - 50)/2, 300, 80);

	//Boolean used to display prompt
	private bool prompt = false;
    
	//Used to trigger full screen enabled.
	private bool enableFull = false;

	void OnGUI () 
	{
        //Restriction should be placed here. Not sure how to go about this I tried using Android argument and it does not show up on my phone(ANDROID) when I build it.
		//NB: 11/25/2017
		//edited conditional to detect ios devices
		if(Application.platform != RuntimePlatform.OSXPlayer || SystemInfo.operatingSystem.ToLower().Contains("ios")){
            if(prompt)
                promptPanel = GUI.Window (0, promptPanel, promptWindow, "Fullscreen Mode Recommended.");
            fullPanel = GUI.Window (1, fullPanel, panelWindow, "Fullscreen Mode: ");
        }
        
	}
		
	// Panel Window
	void panelWindow (int windowID)
	{
		float y = 20;	

		if(GUI.Button(new Rect(5,y, fullPanel.width/3,  50), "ON"))
		{
			//Set Fullscreen
			Screen.SetResolution (Screen.width, Screen.height, true);
			enableFull = true;

		}

		if(GUI.Button(new Rect(180,y, fullPanel.width/3, 50), "OFF"))
		{
			enableFull = false;
            Application.Quit();
            Screen.SetResolution (Screen.width, Screen.height, false);

		}
	}
	// Prompt Window
	void promptWindow (int windowID)
	{
		float y = 20;

		if(GUI.Button(new Rect(promptPanel.width/2-40,y, promptPanel.width/6, 50), "OK"))
		{
            //User selects ok, make the prompt go away.
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
			Screen.SetResolution (Screen.width, Screen.height, true);
	}

}

	

