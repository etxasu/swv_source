using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// This class handles the setDataRequest and getDataRequest functionality of the SPR Transporter.
// Use this to store data locally in-session in case the user wants to refresh their browser.
// Strategically speaking, we are optimizing this procedure for quicker saving and loading.
// It will be a bear to maintain, but our end-users have subpar hardware.
// We MUST make this process as quick and performant for the user as possible, so it'll be harder on us.
// JOS: 12/9/2016

/*

    ||| FoundObjectBit keyguide |||
    
    Example String:
    0,0,0,0,false,false,false,

    Data Stored (in the following order):

    Planets
    Near Earth Objects (NEO) as binary integer
    Main Asteroid Belt (MAB/MBO) as binary integer
    Kuiper Belt Objects (KBO) as binary integer
    NEO Visible in ENV as bool
    MBO Visible in ENV as bool
    KBO Visible in ENV as bool

 */
public class SPR_LocalData : MonoBehaviour
{
    public string FoundObjectBit;
    public string SimID;
    public string KeyName;

    public SceneController MySceneController;
    public FactoidsController MyFactoids;

    public FoundFlags[] LocatedBits = new FoundFlags[4];
    public List<string> KeyList;

    public GameObject PlanetGroup;
    public GameObject KBOGroup;
    public GameObject MABGroup;
    public GameObject NEOGroup;

    public int UpdateLoops = 0;
    public bool CurrentlyLoadingSaveData = false;

	// Use this for initialization
	void Start ()
    {
        MySceneController = gameObject.GetComponent<SceneController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Clunky because we want this to run on the second loop, not first.
	    if(UpdateLoops == 0)
        {
            UpdateLoops++;
        }
        else if(UpdateLoops == 1)
        {
            CheckSaveState();
            UpdateLoops = 2;
        }

	}

    // Make sure the correct transform is passed in
    public void FlipBits(FoundFlags _self, SmallWorldType _type)
    {
        //Debug.Log(_self.ToString() + " | " + _type.ToString());

        switch (_type)
        {
            case SmallWorldType.NEO:
                if(!UserFlagExtensions.HasFlagQuick(LocatedBits[1], _self)) // Make sure it's not already in there
                {
                    LocatedBits[1] += (int)_self;
                }
                //Debug.Log(LocatedBits[1].ToString());
                break;
            case SmallWorldType.MAB:
                if (!UserFlagExtensions.HasFlagQuick(LocatedBits[2], _self)) // Make sure it's not already in there
                {
                    LocatedBits[2] += (int)_self;
                }
                //Debug.Log(LocatedBits[2].ToString());
                break;
            case SmallWorldType.KB:
                if (!UserFlagExtensions.HasFlagQuick(LocatedBits[3], _self)) // Make sure it's not already in there
                {
                    LocatedBits[3] += (int)_self;
                }
                //Debug.Log(LocatedBits[3].ToString());
                break;
            case SmallWorldType.Planet:
                if (!UserFlagExtensions.HasFlagQuick(LocatedBits[0], _self)) // Make sure it's not already in there
                {
                    LocatedBits[0] += (int)_self;
                }
                //Debug.Log(LocatedBits[0].ToString());
                break;
            default:
                Debug.Log("BAD WORLD PASSED INTO FLIP FUNCTION!");
                break;
        }
    }

    public void WriteToSPR()
    {
        string _value = "";

        // Write Worlds, should only go to four
        foreach (FoundFlags _e in LocatedBits)
        {
            _value = _value + ((int)_e).ToString() + ",";
        }

        // Write Visibility States, have 3
        _value = _value + MySceneController.NEOGroup.transform.GetChild(0).GetChild(3).GetComponent<MeshRenderer>().enabled.ToString() + ",";
        _value = _value + MySceneController.MBOGroup.transform.GetChild(0).GetChild(3).GetComponent<MeshRenderer>().enabled.ToString() + ",";
        _value = _value + MySceneController.KBOGroup.transform.GetChild(0).GetChild(3).GetComponent<MeshRenderer>().enabled.ToString() + ",";

        // Write current camera mode, 0 1 or 2
        _value = _value + MySceneController.CurrentCameraMode.ToString() + ",";

        // Write the name of the current target. This should never be null, as there are null-catch cases that will set it to Sun instead.
        _value = _value + MySceneController.SelectedObject + ",";

        Debug.Log(_value);

        FoundObjectBit = _value;

        Application.ExternalCall("storeUnityData", SimID, KeyName, _value);
    }

    public void WipeSaveData()
    {
        Application.ExternalCall("storeUnityData", SimID, KeyName, "0,0,0,0,False,False,False,");
    }

    public void CheckSaveState()
    {
        Application.ExternalCall("getUnityData", SimID, KeyName);
    }

    public void ReadFromSPR(string _data)
    {
        Debug.Log("Checking for existing Save Data.");

        Debug.Log("Save state is : " + _data);

        CurrentlyLoadingSaveData = true;

        string[] _values = _data.Split((new[] { ',' }), StringSplitOptions.RemoveEmptyEntries);

        // Get Camera Mode correct!
        if(_values[8] == null) // first time loading sim
        {
            // Give it a valid thing to do
            _data = "0,0,0,0,false,false,false,0,Sun,";
        }
        else if (_values[8] != "Sun")
        {
            MySceneController.FindSelectedObject(_values[8]);

            MySceneController.CUPCamera.GetComponent<CloseUpCamera>().target = MySceneController._selected.transform;

            SelectOrbitalObject _currentTarget = MySceneController.CUPCamera.GetComponent<CloseUpCamera>().target.GetChild(3).GetComponent<SelectOrbitalObject>();

            MySceneController.CUPCamera.GetComponent<CloseUpCamera>().SetMinAndMaxZoom(_currentTarget.CloseUpMaxDistance, _currentTarget.CloseUpMinDistance, _currentTarget.CloseUpStartPercentage);

            MyFactoids.CurrentDataset = MySceneController._selected.GetComponent<OrbitalMovement>();
            MyFactoids.FixMyFactoids();
        }

        int _CameraMode = Int32.Parse(_values[7]);

        switch (_CameraMode)
        {
            case 0:
                // Do nothing, the sim initializes in this mode.
                break;
            case 1:
                MySceneController.ToggleCUP = true;
                Debug.Log("Attempting to initialize into CUP Mode.");
                break;
            case 2:
                MySceneController.ToggleSSV = true;
                break;
            default:
                // wtf, how did you get here
                Debug.Log("Default reached while attempting to set camera mode. Value: " + _CameraMode.ToString());
                break;
        }

        int _index = 0;

        // Inscrutable, but this needs to be done NOW.
        // JOS: 1/9/2017
        foreach(string _s in _values)
        { 
            if (_index < 4)
            {
                int _i = Int32.Parse(_s);
                LocatedBits[_index] = (FoundFlags)_i;
            }

            _index++;
        }

        #region Toggle ENV Visibility States

        if (Boolean.Parse(_values[4]) == true)
        {
            MySceneController.DrawNEOGroup = true;
        }

        if (Boolean.Parse(_values[5]) == true)
        {
            MySceneController.DrawMBOGroup = true;
        }

        if (Boolean.Parse(_values[6]) == true)
        {
            MySceneController.DrawKBOGroup = true;
        }

        #endregion

        LoadAllWorlds();

        StartCoroutine(HoldUpImLoadingData());
    }

    IEnumerator HoldUpImLoadingData()
    {
        yield return new WaitForSeconds(2.5f);
        CurrentlyLoadingSaveData = false;
        yield return null;
    }

    public void UpdateFoundWorlds(string _obj)
    {
        FoundObjectBit = _obj;
        ReadFromSPR(FoundObjectBit);
    }

    // Use this method to load found worlds
    private void LoadUpWorlds(Transform _WorldSet, int _index)
    {
        foreach(Transform _child in _WorldSet)
        {
            //Debug.Log(_child.name);
            //Debug.Log(LocatedBits[_index]);
            if (_child.GetComponent<OrbitalMovement>() != null)
            {
                if (UserFlagExtensions.Contains(LocatedBits[_index], _child.GetComponent<OrbitalMovement>().MySaveFlag))
                {
                    //Debug.Log(_child.GetComponent<OrbitalMovement>().MySaveFlag.ToString() + " | " + LocatedBits[_index]);
                    Debug.Log(_child.name + " now loading");
                    _child.GetChild(3).GetComponent<SelectOrbitalObject>().AddOrbitalPath = true;
                    _child.GetChild(3).GetComponent<SelectOrbitalObject>().AddNamePlate = true;
                    _child.GetChild(3).GetComponent<SelectOrbitalObject>().Studied = true;
                    Capi.set(_child.name + ".CacheFound", _child.GetChild(3).GetComponent<SelectOrbitalObject>().Studied);
                    _child.GetChild(4).GetChild(1).GetComponent<UpdateTrophyCase>().FoundCache();

                }
            }
        }
    }

    // derpy method, but out of time
    // JOS: 12/13/2016
    private void LoadAllWorlds()
    {
        LoadUpWorlds(PlanetGroup.transform, 0);
        LoadUpWorlds(NEOGroup.transform, 1);
        LoadUpWorlds(MABGroup.transform, 2);
        LoadUpWorlds(KBOGroup.transform, 3);
    }
}

public static class UserFlagExtensions
{
    public static bool HasFlagQuick(this FoundFlags a, FoundFlags b)
    {
        return (a & b) == b;
    }

    public static bool Contains(this FoundFlags keys, FoundFlags flag)
    {
        return (keys & flag) != 0;
    }
}

[Flags]
public enum FoundFlags
{
    NoneFound = 0,
    FirstWorld = 1,
    SecondWorld = 2,
    ThirdWorld = 4,
    FourthWorld = 8,
    FifthWorld = 16,
    SixthWorld = 32,
    SeventhWorld = 64,
    EightWorld = 128
}