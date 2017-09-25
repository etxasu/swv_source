using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitalPathRenderer : MonoBehaviour
{
    public int segments;
    public float PathRadius;

    public float xradius;
    public float yradius;
    public float width;
    public float CatchupOffset;
    public bool UpdateLineSize = false;
    public bool IsTestObjectOrbitalPath = false;

    [SerializeField]
    private bool InitializeColliders;

    public SmallWorldType MySmallWorldType;

    private LineRenderer line;
    public List<Vector3> _linePoints = new List<Vector3>();

    public Transform MyTarget;

    public bool FirstUpdate = false;
    [SerializeField]
    private bool UpdateCapi = false;
    private bool CapiUpdated = false;
    public bool OrientSelf = false;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints();

        transform.Rotate(90, 0, 0);
    }

    void Update()
    {
        if (UpdateCapi)
        {
            ExposeMyCapi();
            CapiUpdated = true;
            UpdateCapi = !UpdateCapi;
        }

        if (UpdateLineSize)
        {
            ResizeLine();
            UpdateLineSize = !UpdateLineSize;
            Debug.Log("Resizing line!");
            Capi.set(gameObject.name + ".ResizeLine", UpdateLineSize);   
        }

        if(OrientSelf)
        {
            transform.LookAt(MyTarget);
            transform.rotation = Quaternion.Euler(90.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + CatchupOffset);
        }

        if (FirstUpdate)
        {
            gameObject.name = MyTarget.name + " Orbital Path";

            FirstUpdate = false;
        }
    }

    void ResizeLine()
    {
        //Debug.Log("SIZIN DA LINES");
        xradius = PathRadius;
        yradius = PathRadius;

        Debug.Log(xradius.ToString() + " | " + yradius.ToString() + " | " + width.ToString());

        this.SetWidth(width);
        ResizeColliders(width);
        CreatePoints();
    }

    // This function should only be called if you are attempting to resize a test object!
    public void ResizeTestObjectLine(float _radius)
    {
        // stupid test
        if(IsTestObjectOrbitalPath)
        {
            xradius = _radius;
            yradius = _radius;
            this.SetWidth(width);
            ResizeColliders(width);
            CreatePoints();
        }
    }

    // This is for selectable orbit lines
    public void ResizeColliders(float _wid)
    {
        foreach(Transform _col in transform)
        {
            BoxCollider col = _col.GetComponent<BoxCollider>();
            col.size = new Vector3(col.size.x, col.size.y, _wid);
        }
    }

    private void AddColliderToLineSegment(Vector3 _startPos, Vector3 _endPos, int _index)
    {
        BoxCollider col = new GameObject("Collider Segment" + _index.ToString()).AddComponent<BoxCollider>();

        //col.gameObject.AddComponent<HoverOverOrbitalLineScript>();

        col.transform.parent = line.transform; // Collider is added as child object of line
        col.gameObject.layer = 9;
        float lineLength = Vector3.Distance(_startPos, _endPos); // length of line
        col.size = new Vector3(lineLength, width, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (_startPos + _endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(_startPos.y - _endPos.y) / Mathf.Abs(_startPos.x - _endPos.x));

        if ((_startPos.y < _endPos.y && _startPos.x > _endPos.x) || (_endPos.y < _startPos.y && _endPos.x > _startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle) + 90;
        col.transform.Rotate(angle, -90, -90);
        //col.transform.Rotate(0, 0, 90);
    }

    void ExposeMyCapi()
    {
        Capi.expose<float>(gameObject.name + ".Radius", () => { return PathRadius; }, (value) => { return PathRadius = value; });
        Capi.expose<float>(gameObject.name + ".Width", () => { return width; }, (value) => { return SetWidth(value); });
        Capi.expose<bool>(gameObject.name + ".ResizeLine", () => { return UpdateLineSize; }, (value) => { return UpdateLineSize = value; });
    }

    // This only sets spherical radii
    // JOS: 8/10/2016
    private float SetRadius(float _x)
    {
        xradius = _x;
        yradius = _x;
        Capi.set(gameObject.name + ".Radius", xradius);
        Debug.Log(gameObject.name + " radius was set to " + xradius.ToString());
        return _x;
    }

    public float SetWidth(float _w)
    {
        width = _w;

        line.startWidth = width;
        line.endWidth = width;

        return _w;
    }

    private void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            Vector3 _pos = new Vector3(x, y, z);

            line.SetPosition(i, _pos);
            _linePoints.Add(_pos);

            angle += (360f / segments);
        }

        if(InitializeColliders)
        { 
            for (int i = 0; i < (_linePoints.Count - 1); i++)
            {
                AddColliderToLineSegment(_linePoints[i], _linePoints[i + 1], i);
            }
            InitializeColliders = false;
        }
    }
}
