﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ToggleFullscreen : MonoBehaviour {

    public Button fullButton;
    bool enableFull = true;
    public Sprite OffSprite;
    public Sprite OnSprite;


    // Use this for initialization
    void Start () {
       // Button btn = fullButton.GetComponent<Button> ();
       // btn.onClick.AddListener(FullOnClick);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                Debug.Log("YOU HAVE HIT OBJECT COLLIDER: "+hit.collider.gameObject.name);
            }
        }
    }
    void OnMouseDown(){
        Debug.Log("MOUSE IS DOWN.");

            if (enableFull)
            {
                Screen.SetResolution(Screen.width, Screen.height, true);
                gameObject.GetComponent<Image>().sprite = OffSprite;
                enableFull = false;
                Debug.Log("FULLSCREEN ON");
                //DestroyImmediate (gameObject);
            }
            else
            {
                //Android
                if (Application.platform == RuntimePlatform.Android)
                {
                    Application.Quit();
                    gameObject.GetComponent<Image>().sprite = OnSprite;
                    enableFull = true;
                    Debug.Log("FULLSCREEN OFF");
                    // DestroyImmediate (gameObject);




                }
                //Other 
                else
                {
                    Screen.SetResolution(Screen.width, Screen.height, false);
                    gameObject.GetComponent<Image>().sprite = OnSprite;
                    enableFull = true;
                    Debug.Log("FULLSCREEN OFF");


                }
            }

    }
   
    /*
   void FullOnClick(){
        if (enableFull) {
            Screen.SetResolution (Screen.width, Screen.height, true);
            //Screen.fullScreen = true;
            gameObject.GetComponent<Image> ().sprite = OffSprite;
            Debug.Log("FULLSCREEN OFF");
            enableFull = false;
        } 
        else {
            //Android
            if (Application.platform == RuntimePlatform.Android) {
                Application.Quit ();
                gameObject.GetComponent<Image>().sprite = OnSprite;
                Debug.Log("FULLSCREEN ON");


                enableFull = true;


            }
            //Other 
            else {
                Screen.SetResolution (Screen.width,Screen.height, false);
                gameObject.GetComponent<Image>().sprite = OnSprite;
                Debug.Log("FULLSCREEN ON");

                enableFull = true;

            }
        }
    }*/
}
