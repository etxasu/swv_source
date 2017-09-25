using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonToAdjustSlider : MonoBehaviour
{
    public float SliderIncrement;
    public bool IsPositive;
    public Slider MySlider;

	// Use this for initialization
	void Start ()
    {
        SliderIncrement = (MySlider.maxValue - MySlider.minValue) / 100.0f;
	    
        if(!IsPositive)
        {
            SliderIncrement = -SliderIncrement;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void IncrementSlider()
    {
        MySlider.value += SliderIncrement;
    }
}
