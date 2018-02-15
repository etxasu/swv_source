using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/*
 * KMJ: 02/08/18 - Fullscreen script that allows user to enter and exit fullscreen mode. 
 * Fullscreen button image is swapped each time button is touched. Utilizes OnPointerDown Event.
 */
public class PointerFullscreen : MonoBehaviour, IPointerDownHandler {
    public Sprite OffSprite;
    public Sprite OnSprite;
    bool enableFull = true;

    public void OnPointerDown(PointerEventData eventData){

        Screen.fullScreen = !Screen.fullScreen;
        if (enableFull)
        {
            gameObject.GetComponent<Image>().sprite = OffSprite;
            enableFull = false;

        }
        else{
            gameObject.GetComponent<Image>().sprite = OnSprite;
            enableFull = true;

        }
    }

	// Use this for initialization
	//NB: 02/15/2018
    //checks if operating system allows fullscreen and removes the button if not
	void Start () {
		if (SystemInfo.operatingSystem.ToLower().Contains("ios") || SystemInfo.operatingSystem.ToLower().Contains("mac")) {
            Destroy(GameObject.Find("Fullscreen"));
            Destroy(GameObject.Find("Fullscreen/Background"));
        }
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
