using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenericCAPIButtonToggle : MonoBehaviour
{
    public string CAPIName;
    public bool ToggleButton;
    public GameObject ObjectToToggle;
    public GameObject UIToToggle;
    public bool ExposeCapi;
    public SoundBoard MySoundBoard;
    public float ClickDelay;

    [SerializeField]
    private Image Blackground;

    [SerializeField]
	private bool AcquireSceneController;
    private bool HaveSound;
    private SceneController MySceneController;

	// Use this for initialization
	void Start ()
    {
		if (AcquireSceneController) 
		{
			MySceneController = GameObject.Find("Scene Controller").GetComponent<SceneController>();
		}

        // We'll be explicit here.
        // JOS: 11/2/2016
        if (MySoundBoard != null)
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
        if(ExposeCapi)
        {
            ExposeMyCapi();
            ExposeCapi = false;
        }

        if(ToggleButton)
        {
            // This following code lives here because the above function can be triggered by the sim or SPR.
            // If it were folded into the above function, we would be making extra CAPI calls to SPR when the
            // sim button is used. The boolean here used to enter this block is ONLY used by SPR.
            // To be explicit, a SIM button can simply call the above function directly.
            // JOS: 8/12/2016
            ToggleObjectActive();
            ToggleButton = !ToggleButton;
            Capi.set("UI." + CAPIName + "Toggle", ToggleButton);
        }
	}

    public void ToggleObjectActive()
    {     
        ObjectToToggle.SetActive(!ObjectToToggle.activeSelf);

        gameObject.GetComponent<Button>().interactable = false;
        Blackground.color = Color.gray;

        if (HaveSound)
        {
            MySoundBoard.PlayCertainClip(2);
        }

        if(UIToToggle != null)
        {
            UIToToggle.SetActive(!UIToToggle.activeSelf);
        }

		if(MySceneController.AllowNextOnCacheCase) // && !ObjectToToggle.activeSelf)
        {
			MySceneController.TriggerCheckEvent();

            MySceneController.AllowNextOnCacheCase = false;
            Capi.set("System.AllowNextOnCacheCase", false);
		}

        StartCoroutine(DelayNextClick());

        //ToggleButton = ObjectToToggle.activeSelf;

        Capi.set("UI." + CAPIName + "Toggle", ToggleButton);
    }

    private IEnumerator DelayNextClick()
    {
        yield return new WaitForSeconds(ClickDelay);

        gameObject.GetComponent<Button>().interactable = true;
        Blackground.color = Color.black;

        yield return null;
    }

    private void ExposeMyCapi()
    {
        Capi.expose<bool>("UI." + CAPIName + "Toggle", () => { return ToggleButton; }, (value) => { return ToggleButton = value; });

    }
}
