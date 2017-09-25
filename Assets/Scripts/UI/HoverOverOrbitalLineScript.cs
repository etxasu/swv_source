using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverOverOrbitalLineScript : MonoBehaviour, ISelectHandler
{
    public string TextToDisplay;
    private MinimapHoverNameplate _Nameplate;
    private Text _NameplateText;
    public bool _DoDisplay = false;

	// Use this for initialization
	void Start ()
    {
        _Nameplate = GameObject.Find("MinimapHoverNameplate").GetComponent<MinimapHoverNameplate>();
        _NameplateText = _Nameplate.GetComponent<Text>();
        _NameplateText.color = Color.clear;
    }
	
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("derp");
    }

    public void MyHover(PointerEventData eventData)
    {

    }

	// Update is called once per frame
	void Update ()
    {
        //FadeText();
	}

    void OnMouseOver()
    {
        transform.GetChild(0).SendMessage("OnMouseEnter");
        Debug.Log("FADIN IN!");
        _DoDisplay = true;
    }

    void OnMouseExit()
    {
        _DoDisplay = false;
    }

    private void FadeText()
    {
        
        if(_DoDisplay)
        {
            _NameplateText.text = TextToDisplay;
            _NameplateText.color = Color.Lerp(_NameplateText.color, Color.white, 0.5f * Time.deltaTime);
        }
        else
        {
            _NameplateText.color = Color.Lerp(_NameplateText.color, Color.clear, 0.5f * Time.deltaTime);
        }
    }    
}
