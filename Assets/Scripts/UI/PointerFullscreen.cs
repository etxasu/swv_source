using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

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
	//void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
