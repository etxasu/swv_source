using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleSounds : MonoBehaviour
{
    public AudioListener MyListener;
    public Text MyLabel;
    public Sprite OffSprite;
    public Sprite OnSprite;

    public GameObject NextButtonKludge;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ToggleAudio()
    {
        if(AudioListener.pause)
        {
            AudioListener.pause = false;
            gameObject.GetComponent<Image>().sprite = OnSprite;
            MyLabel.text = "AUDIO: ON";
        }
        else
        {
            AudioListener.pause = true;
            gameObject.GetComponent<Image>().sprite = OffSprite;
            MyLabel.text = "AUDIO: OFF";
        }

        if(!NextButtonKludge.GetComponent<Button>().interactable)
        {
            StartCoroutine(NextButtonKludge.GetComponent<NextButtonWidget>().TimeOutUserClicks());
            Debug.Log("NextButtonKludgeTriggered");
        }
    }
}
