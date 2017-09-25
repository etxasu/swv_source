using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Draw dem nameplates only when they're on the screen.
// This class is supposed to be initialized during another method
// as part of the instantiation that occurs when a user studies an object.
// IE, CacheFound as written in CAPI.
// JOS: 8/2/2016

public class OrbitalNameplate : MonoBehaviour
{
    public GameObject MyTarget;
    public GameObject MyCamera;
    public GameObject FacingIndicator;
    private RectTransform MyRect;
    [SerializeField]
    private bool _DoDraw;
    [SerializeField]
    private bool _SSVDraw;
    public Text MyText;
    public GameObject DragSurface;
    private Rect DragSurfaceRect;
    private Vector3 _Offset;
    private GameObject MyLineJoint;

    // Use this for initialization
    void Start()
    {
        //MyLineJoint = transform.GetChild(2).gameObject;

        MyRect = gameObject.GetComponent<RectTransform>();

        if (MyText == null)
        {
            MyText = transform.GetChild(0).GetComponent<Text>();
        }

        if (DragSurface != null)
        {
            DragSurfaceRect = DragSurface.GetComponent<Rect>();
        }
        //MyText.text = "This is a new\nline test!";
    }

    // Update is called once per frame
    void Update()
    {
        if ((MyTarget != null) && !_SSVDraw)
        {
            MyRect.position = MyCamera.GetComponent<Camera>().WorldToScreenPoint(MyTarget.transform.position);

            _DoDraw = IsInFrontByAngles(MyTarget);
        }
        else if(MyTarget != null)
        {
            _Offset = new Vector3(transform.parent.parent.position.x + DragSurfaceRect.width, 0.0f, transform.parent.parent.position.z + DragSurfaceRect.height);
            MyRect.position = MyCamera.GetComponent<Camera>().WorldToScreenPoint(MyTarget.transform.position) + transform.parent.parent.position;
        }

        if (_DoDraw)
        {
            if (MyText.enabled)
            {
                // JIC block
            }
            else
            {
                MyText.enabled = true;
            }

        }
        else
        {
            if (MyText.enabled)
            {
                MyText.enabled = false;
            }
            else
            {
                // JIC block
            }
        }

    }

    public bool IsInFront(GameObject _go)
    {

        return Vector3.Dot(MyCamera.transform.forward, transform.InverseTransformPoint(_go.transform.position)) > 0;
    }

    public bool IsInFrontByAngles(GameObject _go)
    {
        float _angle = Vector3.Angle(_go.transform.position, FacingIndicator.transform.position);

        if (_angle < 90)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
