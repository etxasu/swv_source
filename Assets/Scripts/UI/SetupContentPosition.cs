using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetupContentPosition : MonoBehaviour
{
    public Vector3 UpdateToThisPosition;
    public bool FirstUpdate = true;
    public float SliderMax = 0.0f;
    public Slider ScrollSlider;
    public LayoutElement MyLayout;
    public float _rows = 1.0f;
    //private float _lastRows;

    private float FontConstant = 12.97297f;

    private int _fontSize;
    private int _fontPadding;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (FirstUpdate)
        {
            SetPosition();
            FirstUpdate = false;
        }
	}

    public void InitializationPosition(int _fSize, int _fPad)
    {
        _fontPadding = _fPad;
        _fontSize = _fSize;
    }

    public void SetPosition()
    {
        float _height = gameObject.GetComponent<RectTransform>().rect.height;
        _rows = _height / FontConstant;
        //transform.localPosition = UpdateToThisPosition + new Vector3(0.0f, _rows * FontConstant ,0.0f);

        //_lastRows = _rows;

        if(MyLayout)
        {
            MyLayout.minHeight = FontConstant * _rows + (2 * _rows);
        }
        //ScrollSlider.maxValue = ScrollSlider.maxValue + gameObject.GetComponent<RectTransform>().rect.height;
    }
}
