using UnityEngine;
using System.Collections;

public class ZoneFlasher : MonoBehaviour
{
    public float FadeDuration;
    public float FadeRate;
    public bool ToggleZone;
    public bool ForceFadeOut;
    public float FadePercentage;
    public Color FlashColor;
    private Color _OriginalColor;
    private LineRenderer _MyLine;
    public bool CurrentFlashState = false;

    [SerializeField]
    private bool ExposeCapi = false;
    private bool CapiExposed;
    [SerializeField]
    private bool UseSceneControllerData = false;
    [SerializeField]
    private SceneController _MySceneController;

	// Use this for initialization
	void Start ()
    {
        _MyLine = gameObject.GetComponent<LineRenderer>();
        _OriginalColor = Color.black;

        _MyLine.material.renderQueue = 500;
	}

    public IEnumerator FadeOut()
    {
        // Number of ticks to use
        float _fAdjust = FadeDuration / FadeRate;

        for (int i = 0; i <= _fAdjust; i++)
        {
            FlashColor.a -= (_fAdjust/ FadePercentage);

            //Debug.Log(FlashColor.a);

            _MyLine.SetColors(FlashColor, FlashColor);

            yield return new WaitForSeconds(FadeRate);
        }  
    }

    public IEnumerator FadeIn()
    {
        // Number of ticks to use
        float _fAdjust = FadeDuration / FadeRate;

        for (int i = 0; i <= _fAdjust; i++)
        {
            FlashColor.a += (_fAdjust / FadePercentage);

            _MyLine.SetColors(FlashColor, FlashColor);

            yield return new WaitForSeconds(FadeRate);
        }
    }

    public IEnumerator FlashZone()
    {
        if (CurrentFlashState == false)
        {
            StartCoroutine(FadeIn());
            CurrentFlashState = true;
        }
        else // already flashed once
        {
           
        }
        yield return null;
    }

    // Update is called once per frame
    void Update ()
    {
        if (ToggleZone)
        {
            if(UseSceneControllerData)
            {
                _MySceneController.SetCurrentZoneData = true;
            }
            StartCoroutine(FlashZone()); 
            ToggleZone = !ToggleZone;

            if (CapiExposed)
            {
                Capi.set(gameObject.name + ".FlashZone", ToggleZone);
            }
        }

        if (ForceFadeOut && (CurrentFlashState == true))
        {
            StartCoroutine(FadeOut());
            CurrentFlashState = false;
            ForceFadeOut = !ForceFadeOut;
            Capi.set("Globals.FadeZone", ForceFadeOut);
        }
        else if(ForceFadeOut)
        {
            ForceFadeOut = !ForceFadeOut;
            Capi.set("Globals.FadeZone", ForceFadeOut);
        }

        if(ExposeCapi)
        {
            ExposeCAPI();
            ExposeCapi = false;
        }
    }

    private void ExposeCAPI()
    {
        Capi.expose<bool>(gameObject.name + ".FlashZone", () => { return ToggleZone; }, (value) => { return ToggleZone = value; });
        Capi.expose<bool>(gameObject.name + ".FadeZone", () => { return ForceFadeOut; }, (value) => { return ForceFadeOut = value; });

        CapiExposed = true;
    }
}
