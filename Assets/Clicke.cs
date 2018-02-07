using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clicke : MonoBehaviour {
    bool enableFull = true;
    public Sprite OffSprite;
    public Sprite OnSprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
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
