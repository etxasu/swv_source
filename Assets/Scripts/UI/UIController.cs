using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

// The objects in this class expect to be in certain positions.
// This is hard-coded on purpose.
// JOS: 8/2/2016

public class UIController : MonoBehaviour
{
    public GameObject MyNextButton;
    public GameObject ENVUI;
    public GameObject InfoPlatesPrefab;
    public Text LessonLabel;
    public bool ShowNextButton = true;
    public float FadeSpeed;
    //public float CenterMessageFadeTimer;

    [SerializeField]
    private bool DoUpdateCenterScreenMessage;

    public GameObject[] UiElements;

    private bool FirstUpdate = true;

    public bool MiniMapToggle
    {
        get { return UiElements[0].activeSelf; }
        set { ReturnActiveState(value, UiElements[0]); }
    }

    public bool ButtonPanelToggle
    {
        get { return UiElements[1].activeSelf; }
        set { ReturnActiveState(value, UiElements[1]); }
    }

    public bool TrophyCaseToggle
    {
        get { return UiElements[2].activeSelf; }
        set { ReturnActiveState(value, UiElements[2]); }
    }

    public bool ZoomSliderToggle
    {
        get { return UiElements[3].activeSelf; }
        set { ReturnActiveState(value, UiElements[3]); }
    }

    public GameObject CacheCaseEnvToggle;

    // Use this for initialization
    void Start ()
    {
	
	}

    public void AddInfoPlates(GameObject _go, Color _color)
    {
        // Instantiate the Object
        GameObject _newPlate = GameObject.Instantiate(InfoPlatesPrefab);
        _newPlate.transform.SetParent(ENVUI.transform);
        _newPlate.transform.SetAsFirstSibling();

        // Setup its variables
        _newPlate.GetComponent<OrbitalNameplate>().MyCamera = gameObject.GetComponent<SceneController>().ENVCamera;
        _newPlate.GetComponent<OrbitalNameplate>().MyTarget = _go;
        _newPlate.GetComponent<OrbitalNameplate>().FacingIndicator = gameObject.GetComponent<SceneController>().ENVCamera.transform.GetChild(1).gameObject;

        // Customize the text
        _newPlate.transform.GetChild(0).GetComponent<Outline>().effectColor = _color;
        _newPlate.transform.GetChild(0).GetComponent<Text>().text = "" + _go.name.ToUpper() + "\n[ " + _go.GetComponent<OrbitalMovement>().MyObjectType.ToString().ToUpper() + " ]";
    }
    private void ExposeCapi()
    {
        Capi.expose<string>("UI.Lesson Title.Text", () => { return LessonLabel.text; }, (value) => { return LessonLabel.text = value; });

        Capi.expose<bool>("UI.ButtonPanel.Shown", () => { return UiElements[0].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[0].gameObject); });
        Capi.expose<bool>("UI.CacheCase.Shown", () => { return UiElements[1].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[1].gameObject); });
        Capi.expose<bool>("UI.MiniMap.Shown", () => { return UiElements[2].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[2].gameObject); });
        Capi.expose<bool>("UI.ZoomSlider.Shown", () => { return UiElements[3].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[3].gameObject); });

        Capi.expose<bool>("UI.PauseButton.Enabled", () => { return UiElements[4].GetComponent<Button>().interactable; }, (value) => { return SetButtonInteractable(value, UiElements[4].gameObject); });
        Capi.expose<bool>("UI.PlanetToggleButton.Enabled", () => { return UiElements[5].GetComponent<Button>().interactable; }, (value) => { return SetButtonInteractable(value, UiElements[5].gameObject); });
        Capi.expose<bool>("UI.StarfieldToggleButton.Enabled", () => { return UiElements[6].GetComponent<Button>().interactable; }, (value) => { return SetButtonInteractable(value, UiElements[6].gameObject); });
        Capi.expose<bool>("UI.TestObjectDragButton.Enabled", () => { return UiElements[7].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[7].gameObject); });

        Capi.expose<bool>("UI.NEO Counter.Enabled", () => { return UiElements[8].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[8].gameObject); });
        Capi.expose<bool>("UI.MBA Counter.Enabled", () => { return UiElements[9].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[9].gameObject); });
        Capi.expose<bool>("UI.KBO Counter.Enabled", () => { return UiElements[10].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[10].gameObject); });

        Capi.expose<bool>("UI.Next Button.Shown", () => { return ShowNextButton; }, (value) => { return DisableNextButton(value, UiElements[11].gameObject); });
        Capi.expose<string>("UI.Next Button.Text", () => { return UiElements[11].gameObject.GetComponent<NextButtonWidget>().MyLabel.text; }, (value) => { return UiElements[11].gameObject.GetComponent<NextButtonWidget>().MyLabel.text = value; });

        Capi.expose<string>("UI.CenterMessage", () => { return UiElements[14].GetComponent<Text>().text; }, (value) => { return UpdateCenterMessage(value, UiElements[14]); });

        Capi.expose<string>("UI.CenterErrorMessage", () => { return UiElements[16].GetComponent<Text>().text; }, (value) => { return UpdateCenterMessage(value, UiElements[16]); });

        Capi.expose<float>("UI.CenterMessageFadeTimer", () => { return FadeSpeed; }, (value) => { return FadeSpeed = value; });

        Capi.expose<bool>("UI.ShowProgressLabel", () => { return UiElements[15].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[15].gameObject); });

        Capi.expose<bool>("UI.SpeedSlider.Shown", () => { return UiElements[12].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[12].gameObject); });
        Capi.expose<bool>("UI.PauseButton.Shown", () => { return UiElements[13].activeSelf; }, (value) => { return ReturnActiveState(value, UiElements[13].gameObject); });

        //Capi.expose<bool>("UI.CacheCase.ENV Toggle", () => { return CacheCaseEnvToggle.activeSelf; }, (value) => { return ReturnActiveState(value, CacheCaseEnvToggle); });
    }

    #region CAPI assist methods
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

    // JOS: 9/12/2016
    public void TestDisableNext()
    {
        DisableNextButton(!ShowNextButton, UiElements[11]);
    }

    private bool DisableNextButton(bool _value, GameObject _go)
    {
        Debug.Log("Processing value " + _value + " for DisableNextButton");
        _go.GetComponent<Image>().enabled = _value;
        _go.GetComponent<Button>().interactable = _value;
        _go.transform.GetChild(0).GetComponent<Image>().enabled = _value;
        _go.transform.GetChild(1).GetComponent<Text>().enabled = _value;

        ShowNextButton = _value;

        return _value;
    }

    public void SetNextInteractableState()
    {
        MyNextButton.GetComponent<Button>().interactable = !MyNextButton.GetComponent<Button>().interactable;
    }

    public void LoopOutAndIn()
    {
        Application.ExternalCall("TestStateChange");
    }

    #endregion

    public string UpdateCenterMessage(string _message, GameObject _go)
    {
        if (!FirstUpdate)
        {
            _go.GetComponent<Text>().text = _message;

            StartCoroutine(StartCenterMessageFade(_go));

            return _message;
        }

        return _message;
    }

    public IEnumerator StartCenterMessageFade(GameObject _go)
    {
        _go.GetComponent<Text>().CrossFadeAlpha(1.0f, 0.2f, false);

        yield return new WaitForSeconds(FadeSpeed);

        _go.GetComponent<Text>().CrossFadeAlpha(0.0f, 0.5f, false);

        yield return null;
    }

 
    // Update is called once per frame
    void Update ()
    {
        if(FirstUpdate)
        {
            ExposeCapi();

            FirstUpdate = !FirstUpdate;
        }

        if(DoUpdateCenterScreenMessage)
        {
            UpdateCenterMessage("Hi", UiElements[14]);
            DoUpdateCenterScreenMessage = false;
        }
	}
}
