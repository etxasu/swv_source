using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonIconToggle : MonoBehaviour
{
    public bool ToggleToIcon;
    public Image IconToToggle;
    public string TextToToggle;
    public Sprite SecondStateIcon;

    public GameObject SceneController;
    private SceneController _MySceneController;

    private Sprite _OriginalIcon;
    private string _OriginalText;

	// Use this for initialization
	void Start ()
    {
        _MySceneController = SceneController.GetComponent<SceneController>();
        _OriginalIcon = IconToToggle.sprite;
        _OriginalText = transform.GetChild(1).GetComponent<Text>().text;

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(SceneController != null)
        {
            if (_MySceneController.CurrentTime != 0)
            {
                if (ToggleToIcon)
                {
                    IconToToggle.sprite = _OriginalIcon;
                }
                else
                {
                    transform.GetChild(1).GetComponent<Text>().text = _OriginalText;
                }
            }
            else if (_MySceneController.CurrentTime == 0.0f)
            {
                if (ToggleToIcon)
                {
                    IconToToggle.sprite = SecondStateIcon;
                }
                else
                {
                    transform.GetChild(1).GetComponent<Text>().text = TextToToggle;
                }
            }

        }
	}

    public void ToggleIcon()
    {
        if(IconToToggle.sprite == _OriginalIcon)
        {
            IconToToggle.sprite = SecondStateIcon;
        }
        else
        {
            IconToToggle.sprite = _OriginalIcon;
            //Debug.Log("HI");
        }

    }
}
