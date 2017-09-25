using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

public class OpenWebLink : MonoBehaviour
{
    public string MyURL;
    public SoundBoard MySoundBoard;
    public int SoundClipIndex;
    public int WindowWidth;
    public int WindowHeight;
    private bool HaveSound;
    //private bool FirstUpdate = true;

    [DllImport("__Internal")]
    private static extern void OpenWebPage(string url, string name, string _size);

    public void OpenLinkWindow()
    {
        //Application.ExternalCall("OpenExternalAsset", MyURL, WindowWidth, WindowHeight);

        string _size = "width=" + WindowWidth.ToString() + ", height=" + WindowHeight.ToString();

        //Debug.Log(_size);

        if(HaveSound)
        {
            MySoundBoard.PlayCertainClip(SoundClipIndex);
        }
        #if !UNITY_EDITOR
        OpenWebPage(MyURL, "SWV Asset", _size);
        #endif
    }

    public void OpenFullScreenWindow()
    {
        //string _size = "";

        Debug.Log(HaveSound);

        if (HaveSound)
        {
            //Debug.Log("DEERP");
            MySoundBoard.PlayCertainClip(SoundClipIndex);
        }

#if !UNITY_EDITOR
        OpenWebPage(MyURL, "_blank", "");
#endif

    }

    // Use this for initialization
    void Start ()
    {
        if (MySoundBoard != null)
        {
            HaveSound = true;
        }
        else
        {
            HaveSound = false;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
