using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class TogglePanel : MonoBehaviour
{
    public GameObject MyTarget;
    public GameObject[] UIToggles;
    public GameObject[] PlanetToggles;
    //public GameObject[] CUPENVToggles;
    public FirstPersonController CameraController;

    public string IncomingMessage;
    public string OutgoingMessage;
    public string AssessmentTransferMessage;
    public string AssessmentCompleteMessage;

    public string FirstLabel;
    public string SecondLabel;

    public Camera SSVCamera;
    public Camera CUPCamera;
    private Camera ENVCamera;
    public MeshRenderer ViewCone;

    public GameObject UIFader;
    public GameObject FeedbackContainer;
    private Image _UIFaderImage;
    private bool SSVUI_Tracker = false;

    private SceneController MySceneController;

    public float FadeSpeed;

    public bool FirstUpdate;

    

	// Use this for initialization
	void Start ()
    {
        MySceneController = gameObject.GetComponent<SceneController>();

        UIFader.SetActive(true);
        _UIFaderImage = UIFader.GetComponent<Image>();
        FadeIn();
        //UIFader.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        ENVCamera = Camera.main;

        
    }

    public void ExposeCAPI()
    {
        Debug.Log("Updating Panel CAPI");
        
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void SSV_ENVFade()
    {
        if(!SSVUI_Tracker)
        {
            UIFader.transform.GetChild(0).GetComponent<Text>().text = AssessmentTransferMessage;
            MySceneController.CurrentCameraMode = 2;
        }
        else
        {
            UIFader.transform.GetChild(0).GetComponent<Text>().text = AssessmentCompleteMessage;
            MySceneController.CurrentCameraMode = 0;
        }

        SSVUI_Tracker = !SSVUI_Tracker;

        StartCoroutine(FadeOut(SSVCamera, UIToggles));
    }

    // No comment, just flagging. JOS 9/28/2016
    public void CUP_ENVFade()
    {
        //Debug.Log(CUPCamera.GetComponent<CloseUpCamera>().target.GetChild(3).name);
        if (CUPCamera.GetComponent<CloseUpCamera>().target.GetChild(3).GetComponent<SelectOrbitalObject>() != null)
        {
            Debug.Log("<color=blue>" + CUPCamera.GetComponent<CloseUpCamera>().target.name + "</color>");

            if (CUPCamera.GetComponent<CloseUpCamera>().target.GetChild(3).GetComponent<SelectOrbitalObject>().Studied == false)
            {
                //CUPCamera.GetComponent<CloseUpCamera>().ResetZoomLevels();
                StartCoroutine(FadeOut(CUPCamera, UIToggles));
                gameObject.GetComponent<SceneController>().ShowOnlyMostRecentSmallWorldOrbitLine();
                UIFader.transform.GetChild(0).GetComponent<Text>().text = IncomingMessage;

                MySceneController.CurrentCameraMode = 1;

                ViewCone.enabled = false;
            }
            else if(gameObject.GetComponent<SceneController>().SPR_FoundCache == true)
            {
                //CUPCamera.GetComponent<CloseUpCamera>().ResetZoomLevels();
                StartCoroutine(FadeOut(CUPCamera, UIToggles));
                gameObject.GetComponent<SceneController>().ShowAllHiddenSmallWorldOrbitLines();

                MySceneController.CurrentCameraMode = 0;

                UIFader.transform.GetChild(0).GetComponent<Text>().text = OutgoingMessage;
                ViewCone.enabled = true;
            }

            // Toggle off ENV controls
            CameraController.ControlEnabled = !CameraController.ControlEnabled;
        }
    }

    public IEnumerator FadeOut(Camera _CameraToSwap, GameObject[] _tgos)
    {
        UIFader.GetComponent<Image>().CrossFadeAlpha(1.0f, FadeSpeed, true);
        //Debug.Log(UIFader.GetComponent<Image>().ToString());
        UIFader.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(FadeSpeed);
        UIFader.transform.GetChild(0).gameObject.SetActive(false);
        SwapCamera(_CameraToSwap, _tgos);
        FadeIn();
        MySceneController.SaveDataRightNow = true;
        yield return null;
    }

    public void FadeIn()
    {
        //_UIFaderImage.color = Color.Lerp(_UIFaderImage.color, Color.clear, FadeSpeed * Time.deltaTime);
        _UIFaderImage.CrossFadeAlpha(0.0f, FadeSpeed, false);
    }

    // This function assumes you have the togglepanel configured a certain way in the inspector.
    // ENV = 0
    // SSV = 1
    // CUP = 2
    // JOS: 7/21/2016
    public void SwapCamera(Camera _cameraToSwap, GameObject[] _tgos)
    {
        if (_cameraToSwap == CUPCamera)
        {
            if(!_tgos[2].activeSelf)
            {
                // ENV
                _tgos[0].SetActive(false);
                // SSV
                _tgos[1].SetActive(false);
                // CUP
                _tgos[2].SetActive(true);

                ToggleFeedbackContainer(true);
                CUPCamera.enabled = true;
                SSVCamera.enabled = false;
            }
            else
            {
                // ENV
                _tgos[0].SetActive(true);
                // SSV
                _tgos[1].SetActive(false);
                // CUP
                _tgos[2].SetActive(false);

                ToggleFeedbackContainer(true);
                CUPCamera.enabled = false;
                SSVCamera.enabled = false;
            }           
        }
        else if (_cameraToSwap == SSVCamera)
        {
            if (!_tgos[1].activeSelf)
            {
                // ENV
                _tgos[0].SetActive(false);
                // SSV
                _tgos[1].SetActive(true);
                // CUP
                _tgos[2].SetActive(false);

                ToggleFeedbackContainer(false);
                CUPCamera.enabled = false;
                SSVCamera.enabled = true;
            }
            else
            {
                // ENV
                _tgos[0].SetActive(true);
                // SSV
                _tgos[1].SetActive(false);
                // CUP
                _tgos[2].SetActive(false);

                ToggleFeedbackContainer(true);
                CUPCamera.enabled = false;
                SSVCamera.enabled = false;
            }
        }

        //CameraController.ControlEnabled = !CameraController.ControlEnabled;
    }

    public void ToggleFeedbackContainer(bool onOff)
    {
        if(!onOff)
        {
            foreach(Transform _child in FeedbackContainer.transform)
            {
                _child.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Transform _child in FeedbackContainer.transform)
            {
                _child.gameObject.SetActive(true);
            }
        }
    }

    public void ToggleObject()
    {
        MyTarget.SetActive(!MyTarget.activeSelf);
        CameraController.ControlEnabled = !CameraController.ControlEnabled;
    }

    public void ToggleENVPlanetVisbility()
    {
        foreach(GameObject _go in PlanetToggles)
        {
            _go.transform.GetChild(3).gameObject.SetActive(!_go.transform.GetChild(3).gameObject.activeSelf);
            //_go.SetActive(!_go.activeSelf);
        }

    }
}
