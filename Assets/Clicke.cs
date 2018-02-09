using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * KMJ: 02/08/18 - Fullscreen script that allows user to enter and exit fullscreen mode. 
 * Fullscreen button image is swapped each time button is touched. Utilizes OnMouseDown Trigger.
 */
public class Clicke : MonoBehaviour {
    bool enableFull = true;
    public Sprite OffSprite;
    public Sprite OnSprite;

	// Use this for initialization
	void Start () {
		
	}
	
    // KMJ: 02/03/18 Checks for collider hit and tells Unity to start OnMouseDown Trigger.
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                Debug.Log("YOU HAVE HIT OBJECT COLLIDER: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.name == "Fullscreen" ) {
                    hit.collider.SendMessage("OnMouseDown");
                    
                } 
            }
           
        }
		
	}

    // KMJ: 02/03/18 Toggles fullscreen and image each time button is touched.
    public void OnMouseDown()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if (enableFull)
        {
            gameObject.GetComponent<Image>().sprite = OffSprite;
            enableFull = false;

        }
        else
        {
            gameObject.GetComponent<Image>().sprite = OnSprite;
            enableFull = true;

        }
        
    }
   
}
