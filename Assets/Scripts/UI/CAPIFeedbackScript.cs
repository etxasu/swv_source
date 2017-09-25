using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CAPIFeedbackScript : MonoBehaviour
{
    public float FeedbackType; // UpdateFeedbackColorBox(float _value)
    public bool Shown = false;
    public bool ExposeCAPI = true;
    public string MyFeedbackText;
    public string WebContentText;
    public float WebContentWidth;
    public float WebContentHeight;
    public string WebContentURL;
    public Text MyObjectiveText;
    public GenericElementFlasher MyObjectiveFlasher;
    private bool FirstUpdate = true;

    [Header("Content Container Properties")]
    public string CAPI_Name;
    public GameObject ContentContainer;
    public GameObject ContentPrefab;
    public GameObject WebContentPrefab;
    public GameObject ContentSlider;
    public GameObject FirstContent;
    public bool CreateNewContentObject;
    public bool CreateNewWebContentObject;
    private int NumberOfContents;
    private bool _TimestampColorToggle;
    public string[] CAPI_SendMessage;


    [Header("Colors")]
    [SerializeField]
    private Color CorrectFeedbackColor;

    [SerializeField]
    private Color IncorrectFeedbackColor;

    [SerializeField]
    private Color HintFeedbackColor;

    [SerializeField]
    private string TimeStampColor;

    // Use this for initialization
    void Start ()
    {
        //_MyText = transform.GetChild(1).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // This is the correct time to expose CAPI
        if(FirstUpdate)
        {
            if(ExposeCAPI)
            {
                ExposeCapi();
            }
            //ReturnActiveState(false, gameObject);
            FirstUpdate = false;

            //FirstContent.GetComponent<Text>().text = CreateTimeStamp() + FirstContent.GetComponent<Text>().text;

            // Toggle kludge
            transform.GetChild(0).gameObject.SetActive(true);
        }

        if(CreateNewContentObject)
        {
            CreateNewContent();
            CreateNewContentObject = false;
            Capi.set(CAPI_Name +".CreateNewFeedback", CreateNewContentObject);

            //UpdateObjectiveText("DEERP");
        }

        if(CreateNewWebContentObject)
        {
            CreateNewWebLinkContent();
            CreateNewWebContentObject = false;
            Capi.set(CAPI_Name + ".CreateNewWebLink", CreateNewWebContentObject);
        }
	}

    public void UpdateContentContainerPosition()
    {
        Vector3 _position = ContentContainer.transform.localPosition;

        ContentContainer.transform.localPosition = new Vector3(-5.5f, ContentSlider.GetComponent<Slider>().value + 223, _position.z) *-1.0f;
    }

    private void CreateNewContent()
    {
        Transform _lastChild = ContentContainer.transform.GetChild(ContentContainer.transform.childCount - 1);
        GameObject _newContent = Instantiate(ContentPrefab);
        _newContent.transform.position = Vector3.zero;
        _newContent.name = "InstructionalContent" + NumberOfContents.ToString();
        NumberOfContents++;
        _newContent.transform.SetParent(ContentContainer.transform, false);
        _newContent.transform.SetAsFirstSibling();

        SetupContentPosition _Config = _newContent.GetComponent<SetupContentPosition>();
        //SetupContentPosition _LastConfig = _lastChild.GetComponent<SetupContentPosition>();

        //float _yStretch = _lastChild.GetComponent<RectTransform>().rect.height + ContentContainer.GetComponent<RectTransform>().rect.height + ContentContainer.GetComponent<VerticalLayoutGroup>().spacing;

        //ContentContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentContainer.GetComponent<RectTransform>().rect.width, _yStretch);

        //_Config.UpdateToThisPosition = _lastChild.localPosition - new Vector3(0.0f, (_lastChild.GetComponent<RectTransform>().rect.height/2) + (ContentContainer.GetComponent<VerticalLayoutGroup>().spacing * (_LastConfig._rows)), 0.0f);

        _Config.ScrollSlider = ContentSlider.GetComponent<Slider>();
        _Config.ScrollSlider.maxValue += (ContentContainer.GetComponent<VerticalLayoutGroup>().spacing + _lastChild.GetComponent<RectTransform>().rect.height);

        // The CreateTimeStamp() function should include a space at the end.
        _newContent.GetComponent<Text>().text = CreateTimeStamp() + MyFeedbackText;
    }

    // Test functionality
    // JOS: 10/13/2016
    private string ContentViaMessage(int _index, string _message)
    {
        CAPI_SendMessage[_index] = _message;

        if (_message != "init")
        {
            Transform _lastChild = ContentContainer.transform.GetChild(ContentContainer.transform.childCount - 1);
            GameObject _newContent = Instantiate(ContentPrefab);
            _newContent.transform.position = Vector3.zero;
            _newContent.name = "InstructionalContent" + NumberOfContents.ToString();
            NumberOfContents++;
            _newContent.transform.SetParent(ContentContainer.transform, false);
            _newContent.transform.SetAsFirstSibling();

            SetupContentPosition _Config = _newContent.GetComponent<SetupContentPosition>();
            //SetupContentPosition _LastConfig = _lastChild.GetComponent<SetupContentPosition>();

            //float _yStretch = _lastChild.GetComponent<RectTransform>().rect.height + ContentContainer.GetComponent<RectTransform>().rect.height + ContentContainer.GetComponent<VerticalLayoutGroup>().spacing;

            //ContentContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentContainer.GetComponent<RectTransform>().rect.width, _yStretch);

            //_Config.UpdateToThisPosition = _lastChild.localPosition - new Vector3(0.0f, (_lastChild.GetComponent<RectTransform>().rect.height/2) + (ContentContainer.GetComponent<VerticalLayoutGroup>().spacing * (_LastConfig._rows)), 0.0f);

            _Config.ScrollSlider = ContentSlider.GetComponent<Slider>();
            _Config.ScrollSlider.maxValue += (ContentContainer.GetComponent<VerticalLayoutGroup>().spacing + _lastChild.GetComponent<RectTransform>().rect.height);

            // The CreateTimeStamp() function should include a space at the end.
            _newContent.GetComponent<Text>().text = CreateTimeStamp() + CAPI_SendMessage[_index];

            //Capi.set(CAPI_Name + ".SendMessage" + _index.ToString(), CAPI_SendMessage[_index]);
        }
        return CAPI_SendMessage[_index];
    }

    private void CreateNewWebLinkContent()
    {
        Transform _lastChild = ContentContainer.transform.GetChild(ContentContainer.transform.childCount - 1);
        GameObject _newContent = Instantiate(WebContentPrefab);
        _newContent.transform.position = Vector3.zero;
        _newContent.name = "WebContent" + NumberOfContents.ToString();
        NumberOfContents++;
        _newContent.transform.SetParent(ContentContainer.transform, false);
        _newContent.transform.SetAsFirstSibling();

        SetupContentPosition _Config = _newContent.GetComponent<SetupContentPosition>();
        //SetupContentPosition _LastConfig = _lastChild.GetComponent<SetupContentPosition>();

        float _yStretch = _lastChild.GetComponent<RectTransform>().rect.height + ContentContainer.GetComponent<RectTransform>().rect.height + ContentContainer.GetComponent<VerticalLayoutGroup>().spacing;

        ContentContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentContainer.GetComponent<RectTransform>().rect.width, _yStretch);

        //_Config.UpdateToThisPosition = _lastChild.localPosition + new Vector3(0.0f, (_lastChild.GetComponent<RectTransform>().rect.height / 2) + (ContentContainer.GetComponent<VerticalLayoutGroup>().spacing * _LastConfig._rows), 0.0f);
        _Config.ScrollSlider = ContentSlider.GetComponent<Slider>();
        //_Config.InitializePosition();
        _Config.ScrollSlider.maxValue += (ContentContainer.GetComponent<VerticalLayoutGroup>().spacing + _lastChild.GetComponent<RectTransform>().rect.height);

        // The CreateTimeStamp() function should include a space at the end.
        _newContent.transform.GetChild(0).GetComponent<Text>().text = CreateTimeStamp() + WebContentText;

        _newContent.AddComponent<LayoutElement>();
        LayoutElement _Layout = _newContent.GetComponent<LayoutElement>();
        _Config.MyLayout = _Layout;

        _Layout.minHeight = (int)(_newContent.transform.GetChild(0).GetComponent<Text>().fontSize * _Config._rows) + 8;

        OpenWebLink _OWLConfig = _newContent.GetComponent<OpenWebLink>();
        _OWLConfig.MyURL = WebContentURL;
        _OWLConfig.WindowWidth = (int)WebContentWidth;
        _OWLConfig.WindowHeight = (int)WebContentHeight;
    }

    private string CreateTimeStamp()
    {
        DateTime _currentTime = DateTime.Now;

        string _timeStamp = "";

        if(_TimestampColorToggle)
        {
            _timeStamp = "<color=white></color><color=" + TimeStampColor + ">[ " + _currentTime.ToString("HH:mm") + " ]</color><color=white> : </color>";

        }
        else
        {
            _timeStamp = "<color=white></color><color=" + TimeStampColor + ">[ " + _currentTime.ToString("HH:mm") + " ]</color><color=white> : </color>";
        }

        _TimestampColorToggle = !_TimestampColorToggle;

        return _timeStamp;
    }

    private void ExposeCapi()
    {
        //Debug.Log(_MyText);

        Capi.expose<string>(CAPI_Name + ".Text", () => { return MyFeedbackText; }, (value) => { return ReturnFormattedText(value, MyFeedbackText); });
        Capi.expose<string>(CAPI_Name + ".ObjectiveText", () => { return MyObjectiveText.text; }, (value) => { return UpdateObjectiveText(value); });
        Capi.expose<string>(CAPI_Name + ".WebLinkText", () => { return WebContentText; }, (value) => { return WebContentText = value; });
        Capi.expose<string>(CAPI_Name + ".WebLinkURL", () => { return WebContentURL; }, (value) => { return WebContentURL = value; });

        Capi.expose<string>(CAPI_Name + ".SendMessage1", () => { return CAPI_SendMessage[0]; }, (value) => { return ContentViaMessage(0, value); });
        Capi.expose<string>(CAPI_Name + ".SendMessage2", () => { return CAPI_SendMessage[1]; }, (value) => { return ContentViaMessage(1, value); });
        Capi.expose<string>(CAPI_Name + ".SendMessage3", () => { return CAPI_SendMessage[2]; }, (value) => { return ContentViaMessage(2, value); });
        Capi.expose<string>(CAPI_Name + ".SendMessage4", () => { return CAPI_SendMessage[3]; }, (value) => { return ContentViaMessage(3, value); });
        Capi.expose<string>(CAPI_Name + ".SendMessage5", () => { return CAPI_SendMessage[4]; }, (value) => { return ContentViaMessage(4, value); });

        Capi.expose<bool>(CAPI_Name + ".Shown", () => { return Shown; }, (value) => { return ReturnActiveState(value, gameObject); });
        Capi.expose<bool>(CAPI_Name + ".CreateNewFeedback", () => { return CreateNewContentObject; }, (value) => { return CreateNewContentObject = value; });
        Capi.expose<bool>(CAPI_Name + ".CreateNewWebLink", () => { return CreateNewWebContentObject; }, (value) => { return CreateNewWebContentObject = value; });
        Capi.expose<float>(CAPI_Name + ".Type", () => { return FeedbackType; }, (value) => { return UpdateFeedbackBoxColor(value); }); 
        Capi.expose<float>(CAPI_Name + ".Web Content Width", () => { return WebContentWidth; }, (value) => { return WebContentWidth = value; });
        Capi.expose<float>(CAPI_Name + ".Web Content Height", () => { return WebContentHeight; }, (value) => { return WebContentHeight = value; });
    }

    public string UpdateObjectiveText(string _text)
    {
        if (MyObjectiveFlasher.gameObject.activeSelf)
        {
            MyObjectiveFlasher.StartFlash();

            //FeedbackSource.PlayOneShot(FeedbackNoticeSound);

            MyObjectiveText.text = _text;

            //Capi.set("Feedback.ObjectiveText", _text);
        }
        return _text;
    }

    public void TestUpdateObjectiveText()
    {
        string _t = UpdateObjectiveText("DERP A DERP A DERP A");
    }

    // This is used by Unity.
    // The method below this is CAPI capable.
    // I could have done an overload, but this implementaiton is politically convenient.
    // JOS: 8/23/2016
    public void InternalToggleActiveState()
    {
        foreach (Transform _children in transform)
        {
            _children.gameObject.SetActive(!_children.gameObject.activeSelf);
        }

        Shown = !Shown;
    }

    #region CAPI Setter assist methods
    public float UpdateFeedbackBoxColor(float _value)
    {
        int _feedbackCase = (int)_value;

        // how about no, not for now JOS: 10/20/2016
        /*
        switch (_feedbackCase)
        {
            case 0: // Correct
                transform.GetChild(1).GetComponent<Image>().color = CorrectFeedbackColor;
                return _feedbackCase;
            case 1: // Incorrect
                transform.GetChild(1).GetComponent<Image>().color = IncorrectFeedbackColor;
                return _feedbackCase;
            case 2: // Hint
                transform.GetChild(1).GetComponent<Image>().color = HintFeedbackColor;
                return _feedbackCase;
            default: // wtf mang
                Debug.Log("Bad variable received from Feedback CAPI. Please confirm that " + _feedbackCase.ToString() + " was what you intended to use.");
                return _feedbackCase;
        }
        */
        return _feedbackCase;
    }

    public string ReturnFormattedText(string _text, string _textToReplace)
    {
        _textToReplace = _text.Replace("\\n","\n");

        Capi.set(CAPI_Name + ".Text", _textToReplace);

        MyFeedbackText = _textToReplace;

        return _textToReplace;
    }

    private bool ReturnActiveState(bool _value, GameObject _go)
    {
        foreach(Transform _children in transform)
        {
            _children.gameObject.SetActive(_value);
        }

        Shown = !Shown;

        return Shown;
    }

    private bool SetButtonInteractable(bool _value, GameObject _go)
    {
        _go.GetComponent<Button>().interactable = _value;

        return _value;
    }




    #endregion  
}

public enum FeedbackType
{
    Correct = 0,
    Incorrect = 1,
    Advice = 2,
};
