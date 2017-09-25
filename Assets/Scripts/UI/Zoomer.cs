using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class Zoomer : MonoBehaviour
{
    public Camera MyCamera;
    public float ZoomIncrement;
    public Transform MyOrbitalPaths;
    public Slider MyZoomSlider;
    public bool CustomCapiNames;

    [SerializeField]
    private float MinTranslateIn;
    [SerializeField]
    private float MaxTranslateIn;

    private float ZoomScalar;

    public bool FirstUpdate = true;
    public bool SlideZoom = false;
    private bool DoExpose = true;

	// Use this for initialization
	void Start ()
    {
        ZoomScalar = MinTranslateIn;
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

    // Update is called once per frame
    void Update ()
    {
        if(FirstUpdate)
        {
            ResizeOrbitalPaths();
            //ExposeCAPI();
            FirstUpdate = false;
        }

        if(DoExpose)
        {
            ExposeCAPI();
            DoExpose = false;
        }
	}

    private void ExposeCAPI()
    {
        if (!CustomCapiNames)
        {
            Capi.expose<float>("Camera.SSV.Zoom Level", () => { return MyCamera.transform.position.y; }, (value) => { return UpdateSSVCameraPosition(value); });
            //Debug.Log(gameObject.name);
        }
        else
        {
            Capi.expose<float>("Camera."+ gameObject.name +".Zoom Level", () => { return MyCamera.transform.position.y; }, (value) => { return UpdateSSVCameraPosition(value); });
            //Debug.Log(gameObject.name);
        }
    }

    private float UpdateSSVCameraPosition(float _level)
    {
        MyZoomSlider.value = _level;

        MyCamera.transform.position = new Vector3(MyCamera.transform.position.x, _level, MyCamera.transform.position.z);

        return _level;
    }

    public void ResizeOrbitalPaths()
    {
        //Debug.Log("RESIZING LINE");
        foreach(Transform Orbit in MyOrbitalPaths)
        {
            float _width = (transform.position.y / ZoomScalar) * 0.1f;
            //Debug.Log(Orbit.name + " | " + _width);
            Orbit.GetComponent<LineRenderer>().startWidth = _width;
            Orbit.GetComponent<LineRenderer>().endWidth = _width;
            Orbit.GetComponent<OrbitalPathRenderer>().ResizeColliders(_width);
        }
        
    }

    // Zoom in if true, out if false
    // This method adjusts the field of view.
    public void Zoom(bool _d)
    {
        if (_d)
        {
            if (MyCamera.fieldOfView - ZoomIncrement <= 0)
            {
                //Debug.Log("lol nope");
            }
            else
            {
                MyCamera.fieldOfView = MyCamera.fieldOfView - ZoomIncrement;
            }
        }
        else
        {
            if (MyCamera.fieldOfView - ZoomIncrement >= 60)
            {
                //Debug.Log("lol nope");
            }
            else
            {
                MyCamera.fieldOfView = MyCamera.fieldOfView + ZoomIncrement;
            }
        }
        //Debug.Log(MyCamera.fieldOfView);
    }

    // Zoom in if true, out if false
    // This method moves the camera.
    public void TranslateZoom(bool _d)
    {
        if (!SlideZoom)
        {
            MyCamera.transform.position = new Vector3(0.0f, MyZoomSlider.value, 0.0f);
        }
        else
        {
            MyCamera.transform.position = new Vector3(-(MyZoomSlider.value/2), MyZoomSlider.value, 0.0f);
        }

        ResizeOrbitalPaths();
    }


    public void OnFingerDown(Lean.LeanFinger finger)
    {
        // deeerp
    }

    public void OnFingerUp(Lean.LeanFinger finger)
    {
        if (!CustomCapiNames)
        {
            Capi.set("Camera.SSV.Zoom Level", MyZoomSlider.value);
        }
        else
        {
            Capi.set("Camera."+ gameObject.name +".Zoom Level", MyZoomSlider.value);
        }
    }
}
