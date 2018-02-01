using UnityEngine;
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
        Button btn = fullButton.GetComponent<Button> ();
        btn.onClick.AddListener(FullOnClick);
    }

    // Update is called once per frame
    void Update()
    {


    }


    /*
    void OnMouseDown(){

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

    }*/
   
   void FullOnClick(){
        if (enableFull) {
            Screen.SetResolution (Screen.width, Screen.height, true);
            //Screen.fullScreen = true;
            gameObject.GetComponent<Image> ().sprite = OffSprite;
            enableFull = false;
        } 
        else {
            //Android
            if (Application.platform == RuntimePlatform.Android) {
                Application.Quit ();
                gameObject.GetComponent<Image>().sprite = OnSprite;
                enableFull = true;


            }
            //Other 
            else {
                Screen.SetResolution (Screen.width,Screen.height, false);
                gameObject.GetComponent<Image>().sprite = OnSprite;
                enableFull = true;

            }
        }
    }
}
