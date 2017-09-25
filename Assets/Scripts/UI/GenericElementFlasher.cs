using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenericElementFlasher : MonoBehaviour
{
    public ColorBlock MyColors;
    public Image MyImage;

    public bool ExposeCapi = false;
    private bool CapiExposed = false;
    public bool TriggerFlash;

    private AudioSource MySoundClip;
    private bool HaveAudio = false;


    [Header("Size Scaling")]
    public bool DoSizeScale;
    [SerializeField]
    private Vector3 FlashScalar;
    private Vector3 OriginalScale;

    [Header("Position Scaling")]
    public bool DoPositionScale;
    [SerializeField]
    private Vector3 FlashPosition;
    private Vector3 OriginalPosition;

    [Header("Autoflash Controls")]
    public bool DoAutoFlash;
    public float MinFlashWait;
    public float RandomWait;
    [SerializeField]
    private float NextFlash;

    public bool SingleFlash;
    public bool DisableFlash;

    // Use this for initialization
    void Start ()
    {
        MySoundClip = gameObject.GetComponent<AudioSource>();

        OriginalScale = transform.localScale;
        OriginalPosition = transform.localPosition;

        if(MySoundClip != null)
        {
            HaveAudio = true;
        }
    }
	
    void OnEnable()
    {
        MyImage.CrossFadeColor(MyColors.normalColor, MyColors.fadeDuration, true, true);
    }

	// Update is called once per frame
	void Update ()
    {
        if(ExposeCapi)
        {
            ExposeCAPI();
            CapiExposed = true;
            ExposeCapi = false;
        }

        if(TriggerFlash)
        {
            StartFlash();
            TriggerFlash = false;

            if(CapiExposed)
            {
                Capi.set("UI." + gameObject.name + ".TriggerFlash", TriggerFlash);
            }
        }

        if(DisableFlash)
        {
            StartCoroutine(FadeOut());
            DisableFlash = false;
        }

        if (DoAutoFlash)
        {
            StartCoroutine(AutoFlashMagic());
            DoAutoFlash = false;
        }

    }

    public void TurnOffTheFlash()
    {
        StartCoroutine(FadeOut());
    }

    private void ExposeCAPI()
    {
        Capi.expose<bool>("UI." + gameObject.name + ".TriggerFlash", () => { return TriggerFlash; }, (value) => { return TriggerFlash = value; });
    }

    public void StartFlash()
    {
        transform.localScale = FlashScalar;
        transform.localPosition = FlashPosition;

        StartCoroutine(FadeIn());

        if (DoSizeScale)
        {
            StartCoroutine(ScaleIn());
        }

        if (DoPositionScale)
        {
            StartCoroutine(PositionTo());
        }
    }

    public IEnumerator AutoFlashMagic()
    {
        StartFlash();

        //Debug.Log("FLASHIN YA BOSS");

        NextFlash = MinFlashWait + Random.Range(0.0f, RandomWait);

        yield return new WaitForSeconds(NextFlash);

        StartCoroutine(AutoFlashMagic());

        yield return null;
    }

    public IEnumerator PositionTo()
    {
        Vector3 _PositionMod = (FlashPosition - OriginalPosition) / 10.0f;

        int _c = 0;

        while (_c < 10)
        {
            transform.localPosition -= _PositionMod;

            _c++;

            yield return new WaitForSeconds((MyColors.fadeDuration) / 10.0f);
        }

        yield return null;
    }

    public IEnumerator ScaleIn()
    {
        Vector3 _ScaleMod = (FlashScalar - OriginalScale)/10.0f;

        while(transform.localScale != OriginalScale)
        {
            transform.localScale -= _ScaleMod;

            yield return new WaitForSeconds((MyColors.fadeDuration) / 10.0f);
        }

        yield return null;
    }

    public IEnumerator FadeIn()
    {
        //Debug.Log("CROSSING THE FADE");
        MyImage.CrossFadeColor(MyColors.highlightedColor, MyColors.fadeDuration, true, true);

        if (HaveAudio)
        {
            MySoundClip.PlayOneShot(MySoundClip.clip);
        }

        yield return new WaitForSeconds(MyColors.fadeDuration);

        if (!SingleFlash)
        {
            StartCoroutine(FadeOut());
        }
        yield return null;
    }

    public IEnumerator FadeOut()
    {
        MyImage.CrossFadeColor(MyColors.normalColor, MyColors.fadeDuration, true, true);

        yield return null;
    }

}
