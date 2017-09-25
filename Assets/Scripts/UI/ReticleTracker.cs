using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReticleTracker : MonoBehaviour
{
    public GameObject MyTarget;
    public GameObject MyCamera;
    public GameObject FacingIndicator;
    public Sprite BadTargetReticle;
    public bool ResetReticle;
    public bool RepositionReticle = false;
    public Vector3 RepositionOffset;

    public bool FirstUpdate = true;

    private GameObject _OriginalTarget;
    [SerializeField]
    private RectTransform MyRect;
    private bool _DoDraw;
    public Image _MyImage;
    public Text _MyText;
    public Sprite OriginalImage;

	// Use this for initialization
	void Start ()
    {
        _OriginalTarget = MyTarget;
        //MyRect = gameObject.GetComponent<RectTransform>();
        //_MyImage = gameObject.GetComponent<Image>();
        //OriginalImage = gameObject.GetComponent<Image>();
        //_DoDraw = true;
    }

    private void ExposeCAPI()
    {
        Capi.expose<bool>("System.Reset Reticle", () => { return ResetReticle; }, (value) => { return ResetReticle = value; });
    }

    // Update is called once per frame
    void Update ()
    {
        if(FirstUpdate)
        {
            FirstUpdate = false;
            ExposeCAPI();            
        }

        if(MyTarget != null && RepositionReticle)
        {
            MyRect.position = Camera.main.WorldToScreenPoint(MyTarget.transform.position);

            Vector3 _tPos = new Vector3(MyRect.position.x - (RepositionOffset.x), MyRect.position.y - (RepositionOffset.y), MyRect.position.z);

            MyRect.position = _tPos;
        }
        //Debug.DrawLine(MyTarget.transform.position, MyCamera.transform.position);
        //_DoDraw = IsInFront(MyTarget);
        _DoDraw = IsInFrontByAngles(MyTarget);

        if(ResetReticle)
        {
            MyTarget = _OriginalTarget;
            ResetReticle = false;
            Capi.set("System.Reset Reticle", ResetReticle);
        }

        if(_DoDraw)
        {
            if(_MyImage.enabled)
            {
                // JIC block
            }
            else
            {
                _MyImage.enabled = true;
                _MyText.enabled = true;
            }

        }
        else
        {
            if (_MyImage.enabled)
            {
                _MyImage.enabled = false;
                _MyText.enabled = false;
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

        if(_angle < 90)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
