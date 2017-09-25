using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

// This script allows you to drag this GameObject using any finger, as long it has a collider
public class SelectOrbitalObject : MonoBehaviour
{
    public Camera CameraToUse;
    public GameObject MySceneController;
    public SoundBoard MySoundBoard;
    private SceneController _SceneControllerRef;
    public Camera CUPCamera;
    public float CloseUpMinDistance = 0.5f;
    public float CloseUpMaxDistance = 15.0f;
    public float CloseUpStartPercentage = 0.5f;

    private bool FirstUpdate = true;
    private Vector3 _StartingScale;
    public float ScaleFactor = 1.0f;
    [SerializeField]
    private GameObject FacingIndicator;
    private GameObject ResetTarget;

    public FactoidsController MyFactoidController;

    public GameObject Reticle;

    [Header("CAPI Objects")]
    public bool Studied = false;
    public bool AddNamePlate = false;
    private bool AlreadyHaveNameplate = false;
    public bool AddOrbitalPath = false;


    [Header("Instantiated Object Properties")]

    public GameObject OrbitalPathPrefab;
    public GameObject OrbitalPathsParent;
    public Color OrbitalPathStartColor;
    public Color OrbitalPathEndColor;
    public GameObject OrbitalPathNameplatePrefab;

    public float ZoneWidth;
    public float ZoneRadius;

    // This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	
	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger draggingFinger;
	
    void Start()
    {
        MySceneController = GameObject.Find("Scene Controller");
        _SceneControllerRef = MySceneController.GetComponent<SceneController>();
        FacingIndicator = GameObject.Find("Facing Indicator");
        //Debug.Log(gameObject.name);
        _StartingScale = transform.localScale;
        ResetTarget = GameObject.Find("Sun");

        if(Reticle == null)
        {
            Reticle = GameObject.Find("Reticle");
        }
    }

    public void AutoAddInfoPlate()
    {
        if (!AlreadyHaveNameplate)
        {
            //Debug.Log(OrbitalPathStartColor.ToString());
            //string _MyColor = string.Format("#{0:X2}{1:X2}{2:X2}", (int)(OrbitalPathStartColor.r * 255), (int)(OrbitalPathStartColor.g * 255), (int)(OrbitalPathStartColor.b * 255));

            MySceneController.GetComponent<UIController>().AddInfoPlates(transform.parent.gameObject, OrbitalPathStartColor);

            // Planet flagging kludge
            if (transform.parent.GetComponent<OrbitalMovement>().MyObjectType == SmallWorldType.Planet)
            {
                Debug.Log("Flagging world as saved: " + transform.parent.name);
                MySceneController.transform.GetComponent<SPR_LocalData>().FlipBits(transform.parent.GetComponent<OrbitalMovement>().MySaveFlag, transform.parent.GetComponent<OrbitalMovement>().MyObjectType);
            }

            AlreadyHaveNameplate = true;
        }
        else
        {
            Debug.Log("Nameplate already exists for " + transform.parent.name);
        }
    }

    public void CreateOrbitalPath()
    {
        //First, check to see if the path object already exists.
        if (OrbitalPathsParent.transform.Find(gameObject.transform.parent.name + " Orbital Path") != null)
        {
            Debug.Log("Orbital path already exists for " + gameObject.transform.parent.name);
            // do nothing, derp derp derp
        }
        else
        {
            GameObject MyOrbitalPath = Instantiate(OrbitalPathPrefab);
            MyOrbitalPath.transform.parent = OrbitalPathsParent.transform;
            MyOrbitalPath.name = gameObject.transform.parent.name + " Orbital Path";

            OrbitalPathRenderer _MyOPR = MyOrbitalPath.GetComponent<OrbitalPathRenderer>();

            // Exposes CAPI
            _MyOPR.FirstUpdate = true;

            // Sets line graphics. Currently only circles are supported
            MyOrbitalPath.GetComponent<LineRenderer>().SetColors(OrbitalPathStartColor, OrbitalPathEndColor);
            _MyOPR.xradius = transform.parent.GetComponent<OrbitalMovement>().radius;
            _MyOPR.yradius = _MyOPR.xradius;
            MyOrbitalPath.transform.position = transform.parent.GetComponent<OrbitalMovement>().PlanarOffset;
            //_MyOPR.UpdateLineSize = true;

            if(transform.parent.GetComponent<OrbitalMovement>().MyObjectType == SmallWorldType.Planet && (transform.parent.GetChild(2).gameObject.activeSelf == false))
            {
                Transform _namePlate = transform.parent.GetChild(2);
                _namePlate.gameObject.SetActive(true);
            }

            _SceneControllerRef.MinimapCamera.GetComponent<Zoomer>().FirstUpdate = true;

            // Tracks target
            _MyOPR.MyTarget = transform.parent;
            _MyOPR.OrientSelf = true;
            _MyOPR.MySmallWorldType = transform.parent.GetComponent<OrbitalMovement>().MyObjectType;

            // Add this to our short list for orbital line tracking IF you're not a planet
            if (transform.parent.parent.name != "Planet Objects")
            {
                _SceneControllerRef.LocatedSmallWorldsList.Add(MyOrbitalPath);
            }            

            //Debug.Log(transform.parent.name + " | " + transform.parent.GetSiblingIndex());

            MySoundBoard.PlayCertainClip(7);
        }
    }

    void Update()
    {
        if(FirstUpdate)
        {
            //Debug.Log(transform.parent.name + " | " + FirstUpdate.ToString());
            ExposeMyCapi();
            FirstUpdate = !FirstUpdate;
        }

        if(AddNamePlate)
        {
            AutoAddInfoPlate();
            AddNamePlate = !AddNamePlate;
            Capi.set(transform.parent.name + ".AddNameplate", AddNamePlate);
        }

        if(AddOrbitalPath)
        {
            CreateOrbitalPath();
            AddOrbitalPath = !AddOrbitalPath;
            Capi.set(transform.parent.name + ".AddOrbitalPath", AddOrbitalPath);
        }

        ScaleByDistance();
    }

    private void ExposeMyCapi()
    {
        Capi.expose<bool>(transform.parent.name + ".CacheFound", () => { return Studied; }, (value) => { return Studied = value; });
        Capi.expose<bool>(transform.parent.name + ".AddNameplate", () => { return AddNamePlate; }, (value) => { return AddNamePlate = value; });

        Capi.expose<bool>(transform.parent.name + ".AddOrbitalPath", () => { return AddOrbitalPath; }, (value) => { return AddOrbitalPath = value; });

    }

    private void ScaleByDistance()
    {
        transform.localScale = _StartingScale * (Vector3.Distance(Vector3.zero, transform.parent.position)) * ScaleFactor;
    }

    protected virtual void OnEnable()
	{
		// Hook into the OnFingerDown event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		
		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp += OnFingerUp;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook the OnFingerDown event
		Lean.LeanTouch.OnFingerDown -= OnFingerDown;
		
		// Unhook the OnFingerUp event
		Lean.LeanTouch.OnFingerUp -= OnFingerUp;
	}
	
	protected virtual void LateUpdate()
	{
		
	}

    public void OnFingerDown(Lean.LeanFinger finger)
	{
        // Raycast information
        Ray ray = finger.GetRay(CameraToUse);
		RaycastHit hit;

        if (MySceneController.GetComponent<SceneController>().LockTarget == false)
        {
            // Was this finger pressed down on a collider?
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask) == true)
            {
                // Was that collider this one?
                if ((hit.collider.gameObject.transform.parent == gameObject.transform.parent) && (hit.collider.gameObject.layer != 9) && (!Studied))
                {
                    // Set the current finger to this one
                    draggingFinger = finger;

                    _SceneControllerRef._selected = hit.collider.gameObject.transform.parent.gameObject;
                    _SceneControllerRef.SelectedObjectType = MySceneController.GetComponent<SceneController>()._selected.GetComponent<OrbitalMovement>().MyObjectType.ToString();
                    Capi.set("Globals.SelectedObject", MySceneController.GetComponent<SceneController>()._selected.name);
                    Capi.set("Globals.SelectedObjectType", MySceneController.GetComponent<SceneController>()._selected.GetComponent<OrbitalMovement>().MyObjectType.ToString());

                    CUPCamera.gameObject.GetComponent<CloseUpCamera>().SetMinAndMaxZoom(CloseUpMaxDistance, CloseUpMinDistance, CloseUpStartPercentage);

                    Reticle = GameObject.Find("Reticle ENV");

                    Reticle.GetComponent<ReticleTracker>().FacingIndicator.transform.position = MySceneController.GetComponent<SceneController>()._selected.transform.position;

                    if (Studied)
                    {
                        Reticle.transform.GetChild(0).GetComponent<Image>().sprite = Reticle.GetComponent<ReticleTracker>().BadTargetReticle;
                        //Reticle.GetComponent<Image>().color = Color.red;
                        Reticle.GetComponent<RectTransform>().localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        //Debug.Log("CHECK");
                        Reticle.transform.GetChild(0).GetComponent<Image>().sprite = Reticle.GetComponent<ReticleTracker>().OriginalImage;
                        //Reticle.GetComponent<Image>().color = Color.yellow;
                        Reticle.GetComponent<RectTransform>().localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    }

                    RepositionReticle(MySceneController.GetComponent<SceneController>()._selected);

                    //UpdateOrbitalZoneInfo();

                    CUPCamera.gameObject.GetComponent<CloseUpCamera>().target = MySceneController.GetComponent<SceneController>()._selected.transform;
                    MyFactoidController.CurrentDataset = transform.parent.GetComponent<OrbitalMovement>();
                    MyFactoidController.FixMyFactoids();
                    //_SceneControllerRef.ProbeTargetLabel.text = hit.collider.gameObject.transform.parent.gameObject.name;
                    _SceneControllerRef.TriggerCheckEvent();
                }
            }
        }
	}

    private void UpdateOrbitalZoneInfo()
    {
        _SceneControllerRef.AlterOrbitalZoneInfo(ZoneWidth, ZoneRadius, MySceneController.GetComponent<SceneController>()._OrbitalZonePath);
    }

    public void RepositionReticle(GameObject _go)
    {
        if(_go != null)
        {
            //Debug.Log(Reticle.name);
            Reticle.transform.GetChild(0).GetComponent<Image>().enabled = true;
            Reticle.transform.GetChild(1).GetComponent<Text>().enabled = true;
            Reticle.GetComponent<ReticleTracker>().MyTarget = _go;
        }
        else
        {
            Reticle.SetActive(false);
        }
    }
	
	public void OnFingerUp(Lean.LeanFinger finger)
	{
		// Was the current finger lifted from the screen?
		if (finger == draggingFinger)
		{
			// Unset the current finger
			draggingFinger = null;
		}
	}
}