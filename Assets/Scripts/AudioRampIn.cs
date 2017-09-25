using UnityEngine;
using System.Collections;

public class AudioRampIn : MonoBehaviour
{
    public bool RampInOnStart;

    [SerializeField]
    private float FadeInTime;
    private float StartVolume;
    private AudioListener MyListener;

	// Use this for initialization
	void Start ()
    {
        MyListener = gameObject.GetComponent<AudioListener>();

	    if(RampInOnStart)
        {
            StartVolume = AudioListener.volume;
            AudioListener.volume = 0.0f;
        }

	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(RampInOnStart)
        {
            StartCoroutine(RampInAudio());
            RampInOnStart = false;
        }
	}

    private IEnumerator RampInAudio()
    {
        yield return new WaitForSeconds(0.5f);

        float _increment = FadeInTime / 100.0f;

        while(AudioListener.volume < 1.0f)
        {
            yield return new WaitForSeconds(_increment);

            AudioListener.volume += 0.01f;
        }

        yield return null;
    }
}
