using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SWV_SSV_Assessment : MonoBehaviour
{
    [Header("Assessment Variables")]
    public GameObject MyTestObject;
    private OrbitalMovement _MyTestObjectOrbitInfo;
    public Vector2 NEOZoneBoundaries;
    public Vector2 MABZoneBoundaries;
    public Vector2 KBOZoneBoundaries;
    public float AssessmentMode;

    [Header("CAPI Variables")]
    public bool NEOZoneCorrect;
    public bool MABZoneCorrect;
    public bool KBOZoneCorrect;
    public bool ResetAssessmentState;

    [Header("System Variables")]
    public bool FirstUpdate;
    public SceneController MySceneController;
    public SoundBoard MySoundBoard;
    public int SoundClip;

	// Use this for initialization
	void Start ()
    {
        _MyTestObjectOrbitInfo = MyTestObject.GetComponent<OrbitalMovement>();

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(FirstUpdate)
        {
            ExposeCapi();
            FirstUpdate = false;
        }

        if(ResetAssessmentState)
        {
            ResetMyAssessment();
            ResetAssessmentState = false;
            Capi.set("Module 1 Assessment.Reset Assessment", ResetAssessmentState);
        }
	}

    private void ResetMyAssessment()
    {

    }

    // This function is structured in this manner because different assessments may need different arguments/initializationss going forward.
    // In the current (10/7/2016) incarnation, this structure will look awkward. It is intentional.
    // JOS: 10/7/2016
    public void VerifyDistance()
    {
        PerformAssessment(AssessmentMode);
        MySoundBoard.PlayCertainClip(SoundClip);
    }

    private void PerformAssessment(float _testType)
    {
        int _Mode = (int)AssessmentMode;
        
        switch (_Mode)
        {
            case 0: // Default
                Debug.Log("NO ASSESSMENT MODE SET");
                break;
            case 1: // NEO
                if(_MyTestObjectOrbitInfo.radius >= NEOZoneBoundaries.x && _MyTestObjectOrbitInfo.radius <= NEOZoneBoundaries.y)
                {
                    NEOZoneCorrect = true;
                    Capi.set("Module 1 Assessment.NEO Zone Correct", NEOZoneCorrect);
                }
                else
                {
                    //ResetAssessmentState = true;
                }
                break;
            case 2: // MAB
                if (_MyTestObjectOrbitInfo.radius >= MABZoneBoundaries.x && _MyTestObjectOrbitInfo.radius <= MABZoneBoundaries.y)
                {
                    MABZoneCorrect = true;
                    Capi.set("Module 1 Assessment.MAB Zone Correct", MABZoneCorrect);
                }
                else
                {
                    //ResetAssessmentState = true;
                }
                break;
            case 3: // KBO
                if (_MyTestObjectOrbitInfo.radius >= KBOZoneBoundaries.x && _MyTestObjectOrbitInfo.radius <= KBOZoneBoundaries.y)
                {
                    KBOZoneCorrect = true;
                    Capi.set("Module 1 Assessment.KB Zone Correct", KBOZoneCorrect);
                }
                else
                {
                    //ResetAssessmentState = true;
                }
                break;
            default:
                Debug.Log("INVALID ASSESSMENT VALUE");
                break;
        }

        MySceneController.TriggerCheckEvent();
    }

    private void ExposeCapi()
    {
        Capi.expose<float>("Module 1 Assessment.Mode", () => { return AssessmentMode; }, (value) => { return AssessmentMode = value; });
        Capi.expose<bool>("Module 1 Assessment.Reset Assessment", () => { return ResetAssessmentState; }, (value) => { return ResetAssessmentState = value; });
        Capi.expose<bool>("Module 1 Assessment.NEO Zone Correct", () => { return NEOZoneCorrect; }, (value) => { return NEOZoneCorrect = value; });
        Capi.expose<bool>("Module 1 Assessment.MAB Zone Correct", () => { return MABZoneCorrect; }, (value) => { return MABZoneCorrect = value; });
        Capi.expose<bool>("Module 1 Assessment.KB Zone Correct", () => { return KBOZoneCorrect; }, (value) => { return KBOZoneCorrect = value; });

        Capi.expose<string>("Module 1 Assessment.Verify Button Label", () => { return transform.GetChild(1).GetComponent<Text>().text; }, (value) => { return transform.GetChild(1).GetComponent<Text>().text = value; });
    }

    // A crude way of clamping the value.
    // JOS: 10/7/2016
    private float ChangeAssessmentMode()
    {
        AssessmentMode = Mathf.Clamp(AssessmentMode, 0, 3);

        Capi.set("Module 1 Assessment.Mode", AssessmentMode);

        return AssessmentMode;
    }
}
