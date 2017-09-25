using UnityEngine;
using UnityEngine.UI;
using Lean;
using System.Collections.Generic;
using System.Collections;

public class SpeedSlider : MonoBehaviour
{
    public Text ValueLabel;
    public GameObject SceneController;

    private Slider SliderRef;

	// Use this for initialization
	void Start ()
    {
        // Hook into Lean
        Lean.LeanTouch.OnFingerUp += OnFingerUp;

        SliderRef = gameObject.GetComponent<Slider>();

        SliderRef.value = SceneController.GetComponent<SceneController>().CurrentTime;
        //ValueLabel.text = SliderRef.value.ToString() + "x";
        SliderRef.onValueChanged.AddListener(delegate { UpdateWorldTime(); });
        
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void OnFingerUp(Lean.LeanFinger finger)
    {
        Capi.set("Globals.WorldSpeed", SceneController.GetComponent<SceneController>().CurrentTime);
    }

    public void UpdateWorldTime()
    {
        if (ValueLabel != null)
        {
            ValueLabel.text = SliderRef.value.ToString() + "x";
        }

        SceneController.GetComponent<SceneController>().CurrentTime = SliderRef.value;
    }

    public void UpdateWorldTimeViaField()
    {
        //ValueLabel.text = SliderRef.value.ToString() + "x";
        
        SceneController.GetComponent<SceneController>().CurrentTime = float.Parse(gameObject.GetComponent<InputField>().text);
    }
}
