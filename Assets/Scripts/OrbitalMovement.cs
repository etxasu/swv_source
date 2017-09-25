using UnityEngine;
using System.Collections;

public class OrbitalMovement : MonoBehaviour
{
    private SceneController _MySceneController;
    public string MySpecialName;
    public bool FixLocation;
    public bool AutoPeformPeriod;
    public Vector3 StagePosition;
    public bool UpdateStagePosition = false;
    public GameObject OrbitalFocus;
    public enum OrbitType { Simple, Quaternion, Trigonometric };
    public OrbitType MyOrbitType;
    public FoundFlags MySaveFlag;

    public OrbitalPathRenderer MyOrbitalPath;

    private bool FirstUpdate = true;
    [SerializeField]
    private bool ExposeCapi = false;

    public string Factoid1;
    public string Factoid2;
    public string Factoid3;
    public string Factoid4;

    [Header("Simple Orbital Variables")]   
    private Vector3 center;
    public Vector3 axis;
    private Vector3 desiredPosition;
    public float radius;
    public float radiusSpeed;
    public float rotationSpeed;

    [Header("Quaternion Orbital Variables")]
    private Vector3 relativePos;
    public Vector3 PlanarOffset;
    public float OrbitalRadius;

    [Header("Trigonometric Orbit Variables")]
    public float Semimajor;
    public float SemiMinor;
    private float Alpha;
    public float AlphaMod;
    private Vector3 Adjustment = Vector3.zero;
    public Vector2 EllipseCenter;
    private float BiggerAxis;
    private float SmallerAxis;
    private bool EllipticalBend;

    private Vector3 _StartingScale;

    public SmallWorldType MyObjectType;

    private float WorldSpeed;

    void Start()
    {
        _StartingScale = transform.localScale;
        _MySceneController = GameObject.Find("Scene Controller").GetComponent<SceneController>();
        if(AutoPeformPeriod)
        {
            GetWorldSpeed();
            InstantiateOrbitalPeriod();
        }

        if(Semimajor > SemiMinor)
        {
            SmallerAxis = SemiMinor;
            BiggerAxis = Semimajor;
        }
        else
        {
            SmallerAxis = Semimajor;
            BiggerAxis = SemiMinor;
        }

        Initialize();
        //transform.position = (transform.position - center.position).normalized * radius + center.position;
    }

    // Use this when instantiating an object
    public void Initialize()
    {
        OrbitalFocus = GameObject.Find("Sun");

        center = OrbitalFocus.transform.position + PlanarOffset;
    }

    public void ExposeMyCapi()
    {
        Capi.expose<float>(gameObject.name + ".distance", () => { return radius; }, (value) => { return radius = value; });
        Capi.expose<bool>(gameObject.name + ".fixLocation", () => { return FixLocation; }, (value) => { return FixLocation = value; });
        Capi.expose<bool>(gameObject.name + ".UpdatePosition", () => { return UpdateStagePosition; }, (value) => { return UpdateStagePosition = value; });
        Capi.expose<float>(gameObject.name + ".OrbitSpeed", () => { return rotationSpeed; }, (value) => { return rotationSpeed = value; });
        Capi.expose<float>(gameObject.name + ".Position.x", () => { return StagePosition.x; }, (value) => { return StagePosition.x = value; });
        Capi.expose<float>(gameObject.name + ".Position.y", () => { return StagePosition.y; }, (value) => { return StagePosition.y = value; });
        Capi.expose<float>(gameObject.name + ".Position.z", () => { return StagePosition.z; }, (value) => { return StagePosition.z = value; });
    }

    // Update material to provided material
    public void UpdateMaterial(Material _m)
    {
        gameObject.GetComponent<MeshRenderer>().material = _m;
    }

    public void GetWorldSpeed()
    {
        WorldSpeed = _MySceneController.CurrentTime;
        radius = Vector3.Distance(transform.position, center);
        //transform.position.x = 1.0f;
    }
    public void InstantiateOrbitalPeriod()
    {
        float _di = Mathf.Pow((radius/10), -3.0f);
        float _pi = Mathf.Sqrt(_di);

        //Debug.Log(gameObject.name + " " + _di);
        rotationSpeed = _pi;        
    }

    public void UpdateCapi()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Capi.set(gameObject.name + ".distance", radius);
            Capi.set(gameObject.name + ".OrbitSpeed", rotationSpeed);
            Capi.set(gameObject.name + ".UpdatePosition", UpdateStagePosition);
            Capi.set(gameObject.name + ".Position.x", transform.position.x);
            Capi.set(gameObject.name + ".Position.y", transform.position.y);
            Capi.set(gameObject.name + ".Position.z", transform.position.z);
        }
    }

    void Update()
    {
        GetWorldSpeed();

        if (!FixLocation)
        {
            switch (MyOrbitType)
            {
                case OrbitType.Simple:
                    SimpleRotate();
                    break;

                case OrbitType.Quaternion:
                    QuaternionRotate();
                    break;

                case OrbitType.Trigonometric:
                    EllipseRotate();
                    break;

                default:
                    // DERP
                    break;
            }
        }

        if (FirstUpdate)
        {
            if (ExposeCapi)
            {
                ExposeMyCapi();
            }
            FirstUpdate = !FirstUpdate;
        }

        if(UpdateStagePosition)
        {
            AdjustStagePosition();
            GetWorldSpeed();
            InstantiateOrbitalPeriod();      
            UpdateStagePosition = !UpdateStagePosition;

            if(MyOrbitalPath)
            {
                MyOrbitalPath.ResizeTestObjectLine(radius);
            }

            UpdateCapi();            
        }
    }

    public void AdjustStagePosition()
    {
        transform.position = StagePosition;
    }

    private void SimpleRotate()
    {
        //Debug.Log(name + " is " + (Vector3.Distance(transform.position, Camera.main.transform.position) + " away"));
        float movementSpeed = (rotationSpeed * WorldSpeed);

        transform.RotateAround(center, axis, movementSpeed * -Time.deltaTime);

        desiredPosition = (transform.position - center).normalized * radius + center;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * movementSpeed);
    }

    private void QuaternionRotate()
    {
        relativePos = (OrbitalFocus.transform.position + PlanarOffset) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime);
        transform.Translate(0, 0, OrbitalRadius * (Time.deltaTime));
    }

    private void EllipseRotate()
    {
        float movementSpeed = (rotationSpeed * WorldSpeed) / (radius / 2);

        Alpha += 1.0f;

        Adjustment.x = EllipseCenter.x + (Semimajor * Mathf.Cos(Alpha * (AlphaMod * WorldSpeed)));
        Adjustment.y = 0.0f;
        Adjustment.z = EllipseCenter.y + (SemiMinor * Mathf.Sin(Alpha * (AlphaMod * WorldSpeed)));

        // TODO: Add local rotation methods
        //transform.RotateAround(center.position + Adjustment, axis, movementSpeed * Time.deltaTime);

        desiredPosition = ((transform.position) - (center ) ).normalized * radius + (center);

        transform.position = Vector3.MoveTowards(transform.position, (Adjustment), Time.deltaTime * movementSpeed);

    }
}

public enum SmallWorldType
{
    Planet = 0,
    MAB = 1,
    NEO = 2,
    OCO = 3,
    KB = 4,
    Test = 5
};
