using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/*
 * KMJ: 02/08/18 - Fullscreen script that allows user to enter and exit fullscreen mode. 
 * Fullscreen button image is swapped each time button is touched. Utilizes OnPointerDown Event.
 * 
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
            //Application.ExternalCall("SetFullscreen",1);
 
        }
        else{
            gameObject.GetComponent<Image>().sprite = OnSprite;
            enableFull = true;
           // Application.ExternalCall("SetFullscreen", 0);

        }
    }

	// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
