using UnityEngine;
using System.Collections;

public class TestBrowserCommunication : MonoBehaviour
{
    public bool MyMessageParameter;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SendMessageToBrowser()
    {
        Application.ExternalCall("SendMessageToUnity", MyMessageParameter.ToString());

        //Debug.Log(GameObject.Find("CAPI").GetComponent<CapiBehaviour>().ToString());

        //GameObject.Find("CAPI").SendMessage("TestBrowserSend", true);
    }
}
