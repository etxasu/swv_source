using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonRotator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float RotationSpeed;
    public bool RotateClockwise;
    public float TransitionSpeed;
    public bool UseCustomTinting;

    private int _rotateOffset;

    public Image SubImage;

    public ColorBlock MyColors;

	// Use this for initialization
	void Start ()
    {
        //gameObject.GetComponent<Image>().CrossFadeColor(MyColors.normalColor, TransitionSpeed, true, true);

        if (UseCustomTinting)
        {
            SubImage.CrossFadeColor(MyColors.normalColor, TransitionSpeed, true, true);
        }

        if (RotateClockwise)
        {
            _rotateOffset = -1;
        }
        else
        {
            _rotateOffset = 1;
        }
    }

    public void OnPointerEnter(PointerEventData dataName)
    {
        if(UseCustomTinting)
        {
            SubImage.CrossFadeColor(MyColors.highlightedColor, TransitionSpeed, true, true);
            //gameObject.GetComponent<Image>().CrossFadeColor(MyColors.highlightedColor, TransitionSpeed, true, true);
        }
        
    }

    public void OnPointerExit(PointerEventData dataName)
    {
        if (UseCustomTinting)
        {
            SubImage.CrossFadeColor(MyColors.normalColor, TransitionSpeed, true, true);
           //gameObject.GetComponent<Image>().CrossFadeColor(MyColors.normalColor, TransitionSpeed, true, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, (RotationSpeed * _rotateOffset)); 
	}
}
