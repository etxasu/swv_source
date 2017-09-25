using UnityEngine;
using System.Collections;

public class SoundBoard : MonoBehaviour
{
    private AudioSource MySource;
    public AudioClip[] MyClips;

    public float ClipDelay = 1.5f;

    [Header("CAPI")]
    public bool PlaySelectedSound;
    public int ClipIndex;
    [SerializeField]
    private bool DoCapiExpose = true;

    // Use this for initialization
    void Start ()
    {
        MySource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(DoCapiExpose)
        {
            ExposeCAPI();
            DoCapiExpose = false;
        }

        if(PlaySelectedSound)
        {
            PlayClips();
            PlaySelectedSound = false;
            Capi.set(gameObject.name + ".PlaySelectedSound", PlaySelectedSound);
        }

	}

    // This method is for CAPI use
    public void PlayClips()
    {
        if (ClipIndex >= 0 && ClipIndex < MyClips.Length && !MySource.isPlaying)
        {
            MySource.PlayOneShot(MyClips[ClipIndex]);
        }
        else
        {
            Debug.Log("SoundBoard Clip " + ClipIndex.ToString() + " is invalid.");
        }
    }

    // This method is for internal use
    public void PlayCertainClip(int _clipIndex)
    {
        if (_clipIndex >= 0 && _clipIndex < MyClips.Length && !MySource.isPlaying)
        {
            MySource.PlayOneShot(MyClips[_clipIndex]);
        }
        else if(MySource.isPlaying)
        {
            Debug.Log("SoundBoard is currently in use! New sound not played.");
        }
        else
        {
            Debug.Log("SoundBoard Clip " + _clipIndex.ToString() + " is invalid.");
        }
    }

    public void ExposeCAPI()
    {
        Capi.expose<bool>(gameObject.name + ".PlaySelectedSound", () => { return PlaySelectedSound; }, (value) => { return PlaySelectedSound = value; });
        Capi.expose<float>(gameObject.name + ".SelectedSound", () => { return ClipIndex; }, (value) => { return ClipIndex = (int)value; });
        Capi.expose<float>(gameObject.name + ".Volume", () => { return MySource.volume; }, (value) => { return MySource.volume = value; });
    }

    public void DebugJavaScriptData(Object _o)
    {
        if (_o != null)
        {
            Debug.Log(_o.ToString());
        }
        else
        {
            Debug.Log("Javascript data is null");
        }
    }
}
