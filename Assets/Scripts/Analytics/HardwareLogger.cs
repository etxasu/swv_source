using UnityEngine;
//using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;

public class HardwareLogger : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // Make an analytics call to record user hardware.
    // Since we don't want to spend our precious, limited analytics time on known systems we
    // make sure we don't log sessions where we use the Editor.
    // JOS: 7/18/2016
    public void LogHardware()
    {
        //if (Application.platform != RuntimePlatform.WindowsEditor)
        //{ 
        //    Analytics.CustomEvent("userHardware", new Dictionary<string, object>
        //    {
        //        { "UID", SystemInfo.deviceUniqueIdentifier },
        //        { "OS", SystemInfo.operatingSystem },
        //        { "GDN", SystemInfo.graphicsDeviceName },
        //        { "PT", SystemInfo.processorType },
        //        { "MS", SystemInfo.systemMemorySize }
        //    });
        //}
        //else
        //{
        //    Debug.Log(SystemInfo.deviceUniqueIdentifier);
        //    Debug.Log(SystemInfo.graphicsDeviceName);
        //    Debug.Log(SystemInfo.operatingSystem);
        //    Debug.Log(SystemInfo.systemMemorySize);
        //    Debug.Log(SystemInfo.processorType);
        //}
    }
    
}
