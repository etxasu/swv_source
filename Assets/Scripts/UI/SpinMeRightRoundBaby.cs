using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpinMeRightRoundBaby : MonoBehaviour
{
    public bool DoRotate;
    public Vector3 RotationSpeed;
    public string LabelToUse;

    public Color ActiveColor;
    public Color NotActiveColor;

    public GameObject ActiveIndicator;

    public Text DisplayedCoinsLabel;

    public GameObject ToggleOn;
    public GameObject[] ToggleOff;
    public GameObject[] Buttons;
    public SoundBoard MySoundBoard;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(DoRotate)
        {
            transform.Rotate(RotationSpeed);
        }
	}

    public void StartRotation()
    {
        DoRotate = true;
    }
    
    public void EndRotation()
    {
        DoRotate = false;
        transform.localRotation = Quaternion.identity;
    }

    public void ToggleShownCoins()
    {
        ToggleOn.SetActive(true);

        foreach(GameObject _go in Buttons)
        {
            if(_go == gameObject)
            {
                _go.GetComponent<Image>().color = ActiveColor;
            }
            else
            {
                _go.GetComponent<Image>().color = NotActiveColor;
            }
        }

        ActiveIndicator.transform.position = new Vector3(ActiveIndicator.transform.position.x, transform.position.y, ActiveIndicator.transform.position.z);

        MySoundBoard.PlayCertainClip(2);

        DisplayedCoinsLabel.text = LabelToUse; 

        foreach (GameObject _go in ToggleOff)
        {
            if(_go.activeSelf == true)
            {
                _go.SetActive(false);
            }
        }
    }
}
