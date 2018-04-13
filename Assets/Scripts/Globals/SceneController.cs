using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.CrashLog;
using System.Collections;
using System.Collections.Generic;

// This file is intended to be the primary Scene/CAPI controller that we can use to 
// control the simulation's visualization.
// It does a lot of stuff, often redundantly, since it has to operate within the
// browser & sandbox messaging limits.
public class SceneController : MonoBehaviour
{
    // SIM Globals
    public float CurrentTime
    {
        get { return _CurrentTime; }
        set { _CurrentTime = value; }
    }

    public Slider SpeedSlider;

    public bool Starfield
    {
        get { return _starField; }
        set { _starField = value; }
    }

    public string SelectedObject
    {
        get { return _selected.name; }
        set { FindSelectedObject(value); }
    }

    [SerializeField]
    private float TriggerDeferment;

    public GameObject FeedbackContainer;

    [SerializeField]
    private float _CurrentTime;
    private float _RememberTime;
    private bool _starField = false;
    //private string ReporterName = "EditorCrash";
    private bool FirstUpdate = true;
    public GameObject _selected;
    public string SelectedObjectType;

    public Text ProbeTargetLabel;
    [SerializeField]
    private GameObject _skyBoxToggle;
    //private bool _toggledSkybox = false;

    public Color[] ZoneColors;

    public GameObject MyTogglePanel;
    
    public GameObject TestObjectDragMenuObject;

    public Vector3 CameraRotation = Vector3.zero;
    public GameObject ENVCamera;
    public GameObject CUPCamera;
    public GameObject SSVCamera;
    public GameObject MinimapCamera;
    // ENV = 0
    // CUP = 1
    // SSV = 2
    public int CurrentCameraMode;

    [Header("Orbital Path Variables")]
    public GameObject TestObjectPath;
    public GameObject AssessmentOrbits;
    public Transform OrbitalPathsContainer;
    public GameObject OrbitalZoneRenderer;
    public OrbitalPathRenderer _OrbitalZonePath;
    public ZoneFlasher _OrbitalZoneFlasher;
    public bool SetCurrentZoneData;
    public float SetCurrentZoneWidth;
    public float SetCurrentZoneMidpoint;
    public List<GameObject> LocatedSmallWorldsList;
    public GameObject[] AssessmentZoneNameplates;

    [Header("Audio")]
    public GameObject SoundBoard;
    public AudioClip CUPTransitionSound;
    public AudioClip BackgroundMusic;

    [Header("CAPI")]
    public bool SaveDataRightNow;
    public bool TutorialMode = false;
    public bool ToggleSSV = false;
    public bool ToggleCUP = false;
    public bool SPR_FoundCache = false;
    public bool LockTarget;
    private bool DrawTestObjects = false;
    public bool PPToggle;
    public bool AllowSimTriggers;
    public bool AllowDelayedSimTriggers;
    public bool DrawKBOGroup;
    public bool DrawMBOGroup;
    public bool DrawNEOGroup;
	public bool AllowNextOnCacheCase;
    public bool ShowNEOAssessmentLines;
    public bool ShowMABAssessmentLines;
    public bool ShowKBOAssessmentLines;
    public float NextScreenDelay = 0.0f;
    public bool TriggerDelayedNext = false;

    [Header("Small World Group Properties")]
    public GameObject KBOGroup;
    public GameObject MBOGroup;
    public GameObject NEOGroup;

    [Header("Cache Case Variables")]
    public float MBO_Found;
    public float KBO_Found;
    public float NEO_Found;
    public int MaxObjectsToFind = 5;
    public GameObject TrophyCase;

    //public List<GameObject> Minimaps;


    // Use this for initialization
    void Start ()
    {
        //CrashReporting.Init("d8c6c1c3-5b04-4066-9d5e-5712575ddb49", "0.1.0", Application.platform.ToString());
        //DisableMinimaps();

        _OrbitalZoneFlasher = OrbitalZoneRenderer.GetComponent<ZoneFlasher>();
        _OrbitalZonePath = OrbitalZoneRenderer.GetComponent<OrbitalPathRenderer>();
    }

    void OnEnable()
    {

    }

    public void AlterOrbitalZoneInfo(float _width, float _radius, OrbitalPathRenderer _path)
    {
        //Debug.Log(_width + " | " + _radius);
        //Debug.Log(_path.name);

        ReAlterOrbitalZoneInfo(_width, _radius, _path);
    }

    private IEnumerator ReAlterOrbitalZoneInfo(float _width, float _radius, OrbitalPathRenderer _path)
    {
        _path.width = _width;
        _path.xradius = _radius;
        _path.yradius = _radius;
        _path.UpdateLineSize = true;

        yield return new WaitForSeconds(0.25f);

        _path.width = _width;
        _path.xradius = _radius;
        _path.yradius = _radius;
        _path.UpdateLineSize = true;
        
        yield return null;
    }

    // Name things by what they should do. Abstract names are for craven secret mongers.
    // JOS: 10/11/2016
    public void ShowOnlyMostRecentSmallWorldOrbitLine()
    {
        if (LocatedSmallWorldsList.Count != 0)
        {
            GameObject _LastObject = LocatedSmallWorldsList[LocatedSmallWorldsList.Count - 1];

            foreach (GameObject _go in LocatedSmallWorldsList)
            {
                if (_go.GetComponent<OrbitalPathRenderer>().MyTarget.gameObject != _selected)
                {
                    _go.SetActive(false);
                }
            }
        }
    }

    public void ShowAllHiddenSmallWorldOrbitLines()
    {
        if (LocatedSmallWorldsList.Count != 0)
        {
            foreach (GameObject _go in LocatedSmallWorldsList)
            {
                if (!_go.activeSelf)
                {
                    _go.SetActive(true);
                }
            }
        }
    }


    #region CAPI Small World Group Toggles

    // These three functions are pretty much identical because there's not a smart way (yet) to
    // use CAPI to pass parameters to the function. Structured this way, the user in the
    // AELP can just set a single boolean for each group.
    // JOS: 8/12/2016
    public void ToggleKBOGroup()
    {
        if (KBOGroup != null)
        { 
            foreach (Transform _child in KBOGroup.transform)
            {
                _child.GetChild(3).GetComponent<MeshRenderer>().enabled = !_child.GetChild(3).GetComponent<MeshRenderer>().enabled;
                _child.GetChild(3).GetComponent<SphereCollider>().enabled = !_child.GetChild(3).GetComponent<SphereCollider>().enabled;
            }

        }
        else // KBOGroup not assigned
        {
            Debug.Log("KBO group not assigned.");
        }
    }

    public void ToggleMBOGroup()
    {
        if (MBOGroup != null)
        {
            foreach (Transform _child in MBOGroup.transform)
            {
                _child.GetChild(3).GetComponent<MeshRenderer>().enabled = !_child.GetChild(3).GetComponent<MeshRenderer>().enabled;
                _child.GetChild(3).GetComponent<SphereCollider>().enabled = !_child.GetChild(3).GetComponent<SphereCollider>().enabled;
            }

        }
        else // MBOGroup not assigned
        {
            Debug.Log("MBO group not assigned.");
        }
    }

    public void ToggleNEOGroup()
    {
        if (NEOGroup)
        {
            foreach (Transform _child in NEOGroup.transform)
            {
                _child.GetChild(3).GetComponent<MeshRenderer>().enabled = !_child.GetChild(3).GetComponent<MeshRenderer>().enabled;
                _child.GetChild(3).GetComponent<SphereCollider>().enabled = !_child.GetChild(3).GetComponent<SphereCollider>().enabled;
            }

       }
        else // NEOGroup not assigned
        {
            Debug.Log("NEO group not assigned.");
        }
    }

    #endregion

    public void ToggleDTO()
    {
        DrawTestObjects = !DrawTestObjects;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(FirstUpdate)
        {
            // This exposes the parent DOM
            Application.ExternalCall("UnityWantsViewerAccess", true);

            // This exposes CAPI!
            ExposeMyCapi();
            FirstUpdate = false;
        }
        else
        {
            // DERP
        }

        // Despite this variable name, it defers the loading for 1 sec to allow things to synchronize.
        // We don't wait for a callback on this because it ends up queued. It just has to arrive after other messages.
        // JOS: 1/18/2017
        if (SaveDataRightNow) 
        {
            StartCoroutine(DelayedSaveCurrentData());
            SaveDataRightNow = !SaveDataRightNow;
            Capi.set("System.Save Current Data Now", SaveDataRightNow);
        }

		if (SetCurrentZoneData) 
		{
			AlterOrbitalZoneInfo (SetCurrentZoneWidth, SetCurrentZoneMidpoint, _OrbitalZonePath);
			SetCurrentZoneData = !SetCurrentZoneData;
			Capi.set ("UI.Minimap.SetCurrentZone", SetCurrentZoneData);
		}

        if (ToggleSSV)
        {
            MyTogglePanel.GetComponent<TogglePanel>().SSV_ENVFade();
            ToggleSSV = !ToggleSSV;

            ReparentOrbitalLines();

            Capi.set("Globals.ToggleSSV", ToggleSSV);
        }

        if (ToggleCUP)
        {
            if (!CUPCamera.GetComponent<Camera>().enabled)
            {
                //Debug.Log("MAKING ROKKIT NOIZ");
                SoundBoard.GetComponent<AudioSource>().PlayOneShot(CUPTransitionSound);
            }

            MyTogglePanel.GetComponent<TogglePanel>().CUP_ENVFade();
            ToggleCUP = !ToggleCUP;
            
            Capi.set("Globals.ToggleCUP", ToggleCUP);
        }

        if(Starfield)
        {
            _skyBoxToggle.GetComponent<ToggleSkyBox>().ToggleSky();
            Starfield = !Starfield;

            Capi.set("Globals.Starfield", Starfield);
        }

        if(PPToggle)
        {
            PauseGame();
            PPToggle = !PPToggle;
            Capi.set("Globals.Pause", PPToggle);
        }

        if(DrawTestObjects)
        {
            //TestObjectDragMenuObject.GetComponent<DragMe>().MakeENVObjects();
            //DrawTestObjects = false;
            //Capi.setInternal("DrawTestObjects", false);
        }

        if(DrawKBOGroup)
        {
            ToggleKBOGroup();
            DrawKBOGroup = false;
            Capi.set("ENV.ToggleKBOGroup", DrawKBOGroup);
        }

        if(DrawMBOGroup)
        {
            ToggleMBOGroup();
            DrawMBOGroup = false;
            Capi.set("ENV.ToggleMBOGroup", DrawMBOGroup);
        }

        if(DrawNEOGroup)
        {
            ToggleNEOGroup();
            DrawNEOGroup = false;
            Capi.set("ENV.ToggleNEOGroup", DrawNEOGroup);
        }

        if(ShowNEOAssessmentLines)
        {
            ShowAssessmentLinesNEO();
            ShowNEOAssessmentLines = false;
            Capi.set("Module 1 Assessment.Toggle NEO Orbit Lines", ShowNEOAssessmentLines);
        }

        if (ShowMABAssessmentLines)
        {
            ShowAssessmentLinesMAB();
            ShowMABAssessmentLines = false;
            Capi.set("Module 1 Assessment.Toggle MAB Orbit Lines", ShowMABAssessmentLines);
        }

        if (ShowKBOAssessmentLines)
        {
            ShowAssessmentLinesKBO();
            ShowKBOAssessmentLines = false;
            Capi.set("Module 1 Assessment.Toggle KBO Orbit Lines", ShowKBOAssessmentLines);
        }

        if (TriggerDelayedNext)
        {
            TriggerDelayedCheckEvent();
            TriggerDelayedNext = false;
            Capi.set("System.Trigger Delayed Next", TriggerDelayedNext);
        }
        //SwitchSkybox();
    }

    private void ReparentOrbitalLines()
    {
        foreach (GameObject _orbitalLine in LocatedSmallWorldsList)
        {
            OrbitalPathRenderer _childRenderer = _orbitalLine.GetComponent<OrbitalPathRenderer>();

            if (_orbitalLine.gameObject.layer == 9)
            {
                _orbitalLine.gameObject.layer = 13;
                _orbitalLine.transform.SetParent(AssessmentOrbits.transform);
                _orbitalLine.GetComponent<LineRenderer>().enabled = false;
            }
            else
            {
                _orbitalLine.gameObject.layer = 9;
                _orbitalLine.transform.SetParent(OrbitalPathsContainer);
                _orbitalLine.GetComponent<LineRenderer>().enabled = true;
            }                   
        }

        if(TestObjectPath.layer == 9)
        {
            TestObjectPath.layer = 13;
        }
        else
        {
            TestObjectPath.layer = 9;
        }

        MinimapCamera.GetComponent<Zoomer>().ResizeOrbitalPaths();
        SSVCamera.GetComponent<Zoomer>().ResizeOrbitalPaths();
    }
    #region Assessment Line CAPI

    //
    // This code is ornery. It is separated into three different functions so they can be called easily through CAPI.
    // This is far from ideal, but it is more convenient for the end user attempting to adapt the lesson.
    // If CAPI becomes more flexible, we can revisit this structure.
    // JOS: 10/19/2016

    public void ShowAssessmentLinesNEO()
    {
        foreach(GameObject _go in LocatedSmallWorldsList)
        {
            if (_go.GetComponent<OrbitalPathRenderer>().MySmallWorldType == SmallWorldType.NEO)
            {
                _go.GetComponent<LineRenderer>().enabled = true;
            }
        }

        AssessmentZoneNameplates[0].SetActive(true);
    }

    public void ShowAssessmentLinesMAB()
    {
        foreach (GameObject _go in LocatedSmallWorldsList)
        {
            if (_go.GetComponent<OrbitalPathRenderer>().MySmallWorldType == SmallWorldType.MAB)
            {
                _go.GetComponent<LineRenderer>().enabled = true;
            }
        }

        AssessmentZoneNameplates[1].SetActive(true);
    }

    public void ShowAssessmentLinesKBO()
    {
        foreach (GameObject _go in LocatedSmallWorldsList)
        {
            if (_go.GetComponent<OrbitalPathRenderer>().MySmallWorldType == SmallWorldType.KB)
            {
                _go.GetComponent<LineRenderer>().enabled = true;
            }
        }

        AssessmentZoneNameplates[2].SetActive(true);
    }
    #endregion
    void LateUpdate()
    {
        if (DrawTestObjects)
        {
            //Debug.Log("NERF");
            //TestObjectDragMenuObject.MakeENVObjects();
            //DrawTestObjects = false;
            //Capi.set("DrawTestObjects", false);
        }
    }

    public void PauseGame()
    {
        SoundBoard.GetComponent<SoundBoard>().PlayCertainClip(2);

        //Debug.Log(_RememberTime);
        if (_CurrentTime != 0.0f)
        {
            //Debug.Log("Goodbye");
            _RememberTime = _CurrentTime;
            _CurrentTime = 0.0f;
        }
        else
        {
            //Debug.Log("HELLO");
            _CurrentTime = _RememberTime;
        }
        //Debug.Log(Time.timeScale.ToString());
    }


    // This function triggers a check event.
    // As the circumstances you need to check trigger events will vary, this function
    // does not test to see if you have a valid reason to do this. You can disable
    // its use altogether with the AllowSimTriggers bool.
    // JOS : 8/23/2016
    public void TriggerCheckEvent()
    {
        if (AllowSimTriggers)
        {
            TriggerDeferment = 0.0f;
            StartCoroutine(DeferTriggerCheck());
            //Debug.Log("<color=yellow>Triggering check event normally!</color>");
        }
        else
        {
            Debug.Log("Sim not allowed to trigger check events.");

            // Kludge!
            // JOS: 03/16/2017

            if(AllowNextOnCacheCase)
            {
                StartCoroutine(ResetTriggers());
            }
        }
    }

    // ONLY USE THIS FUNCTION IF YOU KNOW WHAT THE SCREEN STATE WILL BE!
    // JOS: 11/8/2016
    public void TriggerCheckEventNOW()
    {
        TriggerDeferment = 0.0f;
        StartCoroutine(DeferTriggerCheck());
        //Debug.Log("<color=green>Triggering check event quickly!</color>");
    }

    public void TriggerDelayedCheckEvent()
    {
        if (AllowDelayedSimTriggers)
        {
            TriggerDeferment = NextScreenDelay;
            StartCoroutine(DeferTriggerCheck());          
            AllowDelayedSimTriggers = false;
            Capi.set("System.AllowDelayedSimTriggers", false);
            Debug.Log("Delayed trigger initiated");
        }
        else
        {
            Debug.Log("Sim not allowed to trigger deferred check events.");
            // Kludge!
            // JOS: 03/16/2017

            if (AllowNextOnCacheCase)
            {
                StartCoroutine(ResetTriggers());
            }
        }
    }

    private IEnumerator ResetTriggers()
    {
        yield return new WaitForSeconds(1.0f);

        AllowSimTriggers = true;

        yield return null;
    }


    private IEnumerator DeferTriggerCheck()
    {
        yield return new WaitForSeconds(TriggerDeferment);

        Application.ExternalCall("SendMessageToUnity", false);

        Debug.Log("Check event triggered by Small Worlds Viewer!");

        SaveCurrentData();

        StartCoroutine(SuspendTriggerEvents());

        yield return null;
    }

    public void SaveCurrentData()
    {
        gameObject.GetComponent<SPR_LocalData>().WriteToSPR();
    }

    private IEnumerator DelayedSaveCurrentData()
    {
        yield return new WaitForSeconds(1.0f);

        SaveCurrentData();

        yield return null;
    }

    // Temporarily disable trigger events.
    // JOS : 8/23/2016
    private IEnumerator SuspendTriggerEvents()
    {
        //Debug.Log("SuspendTriggerEvents()");
        AllowSimTriggers = false;
        Capi.set("System.AllowSimTriggers", AllowSimTriggers);
        
        yield return new WaitForSeconds(1.0f);

        AllowSimTriggers = true;
        AllowDelayedSimTriggers = true;
        Capi.set("System.AllowSimTriggers", AllowSimTriggers);
        Capi.set("System.AllowDelayedSimTriggers", AllowDelayedSimTriggers);

        yield return null;
    }

    #region CAPI Setup
    public void ExposeMyCapi()
    {
        // EXPOSE ALL NUMBERS AS FLOATS! INCLUDING INTEGERS! THEY ARE NOW EXPOSED AS FLOATS!
        // JOS: 7/29/2016

        Capi.expose<float>("Globals.WorldSpeed", () => { return CurrentTime; }, (value) => { return UpdateWorldSpeedAndSlider(value); });
        //Capi.expose<bool>("Paused", () => { return Paused; }, (value) => { return Paused = value; });
        Capi.expose<string>("Globals.SelectedObject", () => { return SelectedObject; }, (value) => { return SelectedObject = value; });
        Capi.expose<string>("Globals.SelectedObjectType", () => { return SelectedObjectType; }, (value) => { return SelectedObjectType = value; });
        Capi.expose<bool>("Globals.Starfield", () => { return _starField; }, (value) => { return _starField = value; });
        Capi.expose<bool>("Globals.SPR FoundCache", () => { return SPR_FoundCache; }, (value) => { return SPR_FoundCache = value; });
        Capi.expose<bool>("Globals.ToggleSSV", () => { return ToggleSSV; }, (value) => { return ToggleSSV = value; });
        Capi.expose<bool>("Globals.ToggleCUP", () => { return ToggleCUP; }, (value) => { return ToggleCUP = value; });
        Capi.expose<bool>("Globals.LockTarget", () => { return LockTarget; }, (value) => { return LockTarget = value; });

        Capi.expose<bool>("System.Save Current Data Now", () => { return SaveDataRightNow; }, (value) => { return SaveDataRightNow = value; });
		Capi.expose<bool>("System.AllowNextOnCacheCase", () => { return AllowNextOnCacheCase; }, (value) => { return AllowNextOnCacheCase = value; });
        Capi.expose<bool>("System.Trigger Delayed Next", () => { return TriggerDelayedNext; }, (value) => { return TriggerDelayedNext = value; });
        Capi.expose<float>("System.Delayed Next Duration", () => { return NextScreenDelay; }, (value) => { return NextScreenDelay = value; });

        Capi.expose<bool>("ENV.ToggleKBOGroup", () => { return DrawKBOGroup; }, (value) => { return DrawKBOGroup = value; });
        Capi.expose<bool>("ENV.ToggleMBOGroup", () => { return DrawMBOGroup; }, (value) => { return DrawMBOGroup = value; });
        Capi.expose<bool>("ENV.ToggleNEOGroup", () => { return DrawNEOGroup; }, (value) => { return DrawNEOGroup = value; });

        Capi.expose<bool>("Module 1 Assessment.Toggle NEO Orbit Lines", () => { return ShowNEOAssessmentLines; }, (value) => { return ShowNEOAssessmentLines = value; });
        Capi.expose<bool>("Module 1 Assessment.Toggle MAB Orbit Lines", () => { return ShowMABAssessmentLines; }, (value) => { return ShowMABAssessmentLines = value; });
        Capi.expose<bool>("Module 1 Assessment.Toggle KBO Orbit Lines", () => { return ShowKBOAssessmentLines; }, (value) => { return ShowKBOAssessmentLines = value; });

        Capi.expose<bool>("Globals.Pause", () => { return PPToggle; }, (value) => { return PPToggle = value; });
        Capi.expose<bool>("Globals.FadeZone", () => { return _OrbitalZoneFlasher.ForceFadeOut; }, (value) => { return _OrbitalZoneFlasher.ForceFadeOut = value; });
        Capi.expose<bool>("System.AllowSimTriggers", () => { return AllowSimTriggers; }, (value) => { return AllowSimTriggers = value; });
        Capi.expose<bool>("System.AllowDelayedSimTriggers", () => { return AllowDelayedSimTriggers; }, (value) => { return AllowDelayedSimTriggers = value; });
        Capi.expose<bool>("System.TutorialMode", () => { return TutorialMode; }, (value) => { return TutorialMode = value; });

        Capi.expose<float>("Globals.KBOs Found", () => { return KBO_Found; }, (value) => { return KBO_Found = value; });
        Capi.expose<float>("Globals.MBOs Found", () => { return MBO_Found; }, (value) => { return MBO_Found = value; });
        Capi.expose<float>("Globals.NEOs Found", () => { return NEO_Found; }, (value) => { return NEO_Found = value; });

        // Camera CAPI
        Capi.expose<float>("Camera.Rotation.x", () => { return CameraRotation.x; }, (value) => { return CameraRotation.x = value; });
        Capi.expose<float>("Camera.Rotation.y", () => { return CameraRotation.y; }, (value) => { return CameraRotation.y = value; });

        Capi.expose<bool>("UI.Minimap.FlashZone", () => { return TestObjectDragMenuObject.GetComponent<DragMe>().ShowZone; }, (value) => { return TestObjectDragMenuObject.GetComponent<DragMe>().ShowZone = value; });
		Capi.expose<bool>("UI.Minimap.SetCurrentZone", () => { return SetCurrentZoneData; }, (value) => { return SetCurrentZoneData = value; });
		Capi.expose<float>("UI.Minimap.SetCurrentZoneWidth", () => { return SetCurrentZoneWidth; }, (value) => { return SetCurrentZoneWidth = value; });
		Capi.expose<float>("UI.Minimap.SetCurrentZoneMidpoint", () => { return SetCurrentZoneMidpoint; }, (value) => { return SetCurrentZoneMidpoint = value; });

    }

    public float UpdateWorldSpeedAndSlider(float _speed)
    {
        CurrentTime = _speed;

        SpeedSlider.value = _speed;

        Capi.set("Globals.WorldSpeed", _speed);

        return _speed;
    }

    #endregion

    public void FindSelectedObject(string objectName)
    {
        Debug.Log("Looking for " + objectName);
        _selected = GameObject.Find(objectName);

        if (objectName != "Sun")
        { 
            SelectedObjectType = _selected.GetComponent<OrbitalMovement>().MyObjectType.ToString();
        }

        if (_selected == null)
        {
            Debug.Log("NO GAME OBJECT BY THAT NAME");
        }
        else
        {
            Debug.Log("Found an object.");
            
            // kludge JOS: 12/16/2016
            GameObject.Find("Reticle ENV").GetComponent<ReticleTracker>().MyTarget = GameObject.Find("Sun");
        }
    }

    public void ConsoleTest()
    {
        Application.ExternalCall("onCapiChange");
    }

    public void TestSetData()
    {
        Application.ExternalCall("storeUnityData", "testId", "testKey", "testValue");
    }

    public void TestGetData()
    {
        Application.ExternalCall("getUnityData", "testId", "testKey");
    }

    //public void DisableMinimaps()
    //{
    //    foreach(GameObject _mr in Minimaps)
    //    {
    //        Transform MiniMr = _mr.transform.GetChild(0);
    //        Transform NamePlate = _mr.transform.GetChild(2);
    //        MiniMr.GetComponent<MeshRenderer>().enabled = !MiniMr.GetComponent<MeshRenderer>().enabled;
    //        NamePlate.GetComponent<Canvas>().enabled = !NamePlate.GetComponent<Canvas>().enabled;
    //    }
    //}
}
