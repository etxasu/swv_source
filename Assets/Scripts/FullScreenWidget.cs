using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class FullScreenWidget : MonoBehaviour {

    public bool AllowClicks = true;
    public bool HaveSound = true;
    public float ClickTimeout;
    public SoundBoard MySoundBoard;
    public Image ImageComponent;
    public Sprite OffSprite;
    public Sprite OnSprite;

    private bool fullScreenOn = false;

    public IEnumerator TimeOutUserClicks()
    {
        gameObject.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(ClickTimeout);

        AllowClicks = true;
        gameObject.GetComponent<Button>().interactable = true;

        yield return null;
    }

    public void TriggerFullScreenEvent()
    {
        if (AllowClicks)
        {
            
            AllowClicks = false;
            StartCoroutine(TimeOutUserClicks());

            if (HaveSound)
            {
                MySoundBoard.PlayCertainClip(2);
            }

            if(fullScreenOn)
            {
                Screen.fullScreen = !Screen.fullScreen;
                ImageComponent.overrideSprite = OffSprite;
                fullScreenOn = false;
            }
            else if(!fullScreenOn && Application.platform == RuntimePlatform.Android)
            {
                Application.Quit();
                ImageComponent.overrideSprite = OnSprite;
                fullScreenOn = true;
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                ImageComponent.overrideSprite = OnSprite;
                fullScreenOn = true;
            }
            /* //***Mobile***
            //Android
            if (Application.platform == RuntimePlatform.Android)
            {
                Screen.fullScreen = !Screen.fullScreen;
                //Application.Quit();
                gameObject.GetComponent<Image>().sprite = OnSprite;
                fullScreenOn = true;
            }
            //Other 
            else if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.fullScreen = !Screen.fullScreen;
                //Screen.SetResolution(Screen.width, Screen.height, false);
                gameObject.GetComponent<Image>().sprite = OnSprite;
                fullScreenOn = true;

            }
            */
        }
    }

    private bool ReturnActiveState(bool _value, GameObject _go)
    {
        _go.SetActive(_value);

        return _value;
    }

    private bool SetButtonInteractable(bool _value, GameObject _go)
    {
        _go.GetComponent<Button>().interactable = _value;

        return _value;
    }
}
