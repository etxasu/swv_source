using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartButtonWidget : MonoBehaviour
{
    public Text MyLabel;
    public Image MyImage;
    public bool AllowClicks = true;
    public float ClickTimeout;
    public GameObject MySceneController;
    public string ResetURL;
    private SceneController _MySceneController;
    private bool FirstUpdate = true;
    [SerializeField]
    private Color _OriginalColor;
    public SoundBoard MySoundBoard;
    public bool AttemptRestart;
    public bool SimWantsToRestart;
    private bool HaveSound;

    // Use this for initialization
    void Start ()
    {
        _MySceneController = MySceneController.GetComponent<SceneController>();

        // We'll be explicit here.
        // JOS: 11/2/2016
        if(MySoundBoard != null)
        {
            HaveSound = true;
        }
        else
        {
            HaveSound = false;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(FirstUpdate)
        {
            ExposeMyCapi();
            FirstUpdate = false;
        }
	
        if(AttemptRestart)
        {
            RestartLesson();
            MySceneController.GetComponent<SPR_LocalData>().WipeSaveData();
            AttemptRestart = false;
        }

	}

    IEnumerator TimeOutUserClicks()
    {   
        gameObject.GetComponent<Button>().interactable = false;
        gameObject.transform.GetChild(0).GetComponent<Image>().color = gameObject.GetComponent<Button>().colors.disabledColor;

        yield return new WaitForSeconds(ClickTimeout);
       
        AllowClicks = true;
        gameObject.GetComponent<Button>().interactable = true;

        if (gameObject.transform.GetChild(0))
        {
            gameObject.transform.GetChild(0).GetComponent<Image>().color = _OriginalColor;
        }

        yield return null;
    }

    public void TriggerNextEvent()
    {
        if(AllowClicks)
        {
            AllowClicks = false;
            StartCoroutine(TimeOutUserClicks());

            if (HaveSound)
            {
                MySoundBoard.PlayCertainClip(2);
            }

            _MySceneController.TriggerCheckEventNOW();
        }
    }

    public void InitiateSimRestart()
    {
        SimWantsToRestart = true;

        Capi.set("System.Restart.Sim Wants To Restart", SimWantsToRestart);
        //MyImage.CrossFadeColor(gameObject.GetComponent<Button>().colors.pressedColor, 0.25f, false, false);
        //gameObject.GetComponent<Button>().interactable = false;

        StartCoroutine(DelayTheRestart());
    }

    private IEnumerator DelayTheRestart()
    {
        yield return new WaitForSeconds(0.5f);

        TriggerNextEvent();

        yield return null;
    }

    public void RestartLesson()
    {
        Application.ExternalCall("UnityOpenRestartMenu", 0);
        //SceneManager.LoadScene(0);
    }

    public void OpenSPROptionsMenu()
    {
        Application.ExternalCall("UnityOpenRestartMenu", 0);
    }

    private void ExposeMyCapi()
    {
        Capi.expose<bool>("System.Restart.Sim Wants To Restart", () => { return SimWantsToRestart; }, (value) => { return SimWantsToRestart = value; });
        Capi.expose<bool>("System.Restart.Attempt Restart", () => { return AttemptRestart; }, (value) => { return AttemptRestart = value; });
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
