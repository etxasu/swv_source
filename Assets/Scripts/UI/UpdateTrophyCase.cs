using UnityEngine;
using UnityEngine.UI;
using SmartSparrow;
using System.Collections;

public class UpdateTrophyCase : MonoBehaviour
{
    public GameObject SceneController;
    public ReticleTracker MyReticle;
    public GameObject ClearReticleTarget;
    public GameObject ENVUI;
    public GameObject ENVCamera;
    public GameObject MyCoinCaseSlot;
    public Transform CacheCase;
    public Image SubImage;
    public Material MyCoinCaseImage;
    public Sprite My2DCoinCaseImage;
    private Image _MyCoinSlotImageRef;
    private Text _MyCoinSlotLabelRef;
    public GameObject InfoPlatesPrefab;
    private SceneController _MySceneController;
    public SmallWorldType MyType;
    public GameObject ParticlePrefab;

	// Use this for initialization
	void Start ()
    {
        //_MyCoinSlotImageRef = MyCoinCaseSlot.transform.GetChild(0).GetComponent<MeshRenderer>();
        _MyCoinSlotImageRef = MyCoinCaseSlot.transform.GetChild(2).GetComponent<Image>();
        _MyCoinSlotLabelRef = MyCoinCaseSlot.transform.GetChild(1).GetComponent<Text>();

        _MySceneController = SceneController.GetComponent<SceneController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public IEnumerator BlowUpCoinCache()
    {
        //Debug.Log("HI");
        GameObject _newParticles = GameObject.Instantiate(ParticlePrefab);
        _newParticles.transform.position = gameObject.transform.position;
        gameObject.GetComponent<ButtonRotator>().UseCustomTinting = false;
        //SubImage.color = gameObject.GetComponent<Button>().colors.disabledColor;
        SubImage.CrossFadeColor(gameObject.GetComponent<Button>().colors.disabledColor, 0.1f, true, true);
        
        yield return new WaitForSeconds(1.0f);

        Destroy(_newParticles);

        yield return null;
    }

    public IEnumerator SetStudied()
    {
        DisableMyCache();
        _MySceneController.SPR_FoundCache = true;
        Capi.set("Globals.SPR FoundCache", _MySceneController.SPR_FoundCache);
        yield return new WaitForSeconds(0.5f);
        AddInfoPlates();
        UpdateCoinCaseData();

        if(_MySceneController._selected.transform.GetChild(3).gameObject.GetComponent<SelectOrbitalObject>() != null)
        {
            _MySceneController._selected.transform.GetChild(3).gameObject.GetComponent<SelectOrbitalObject>().Studied = true;

            if (_MySceneController._OrbitalZoneFlasher.CurrentFlashState == true)
            {
                _MySceneController._OrbitalZoneFlasher.StartCoroutine(_MySceneController._OrbitalZoneFlasher.FadeOut());
                _MySceneController._OrbitalZoneFlasher.CurrentFlashState = false;
            }

            Capi.set(transform.parent.parent.name + ".CacheFound", _MySceneController._selected.transform.GetChild(3).gameObject.GetComponent<SelectOrbitalObject>().Studied);

            _MySceneController.transform.GetComponent<SPR_LocalData>().FlipBits(transform.parent.parent.GetComponent<OrbitalMovement>().MySaveFlag, transform.parent.parent.GetComponent<OrbitalMovement>().MyObjectType);
        }

        // Only do this if we're not currently loading.
        if (_MySceneController.gameObject.GetComponent<SPR_LocalData>().CurrentlyLoadingSaveData == false)
        {
            //Debug.Log("<color=green>Triggering check event quickly!</color>");
            _MySceneController.TriggerCheckEventNOW();
        }
        else
        {
            //Debug.Log("<color=red>TriggerCheckEventNow() blocked by kludge!</color>");
        }
        //Application.ExternalCall("SendMessageToUnity", true);
        yield return null;
    }

    // This is where the coin case actually gets updated.
    // JOS: I forget when
    private void UpdateCoinCaseData()
    {
        //_MyCoinSlotLabelRef.text = transform.parent.parent.name;
        _MyCoinSlotLabelRef.text = "";
        _MyCoinSlotImageRef.sprite = My2DCoinCaseImage;
        _MyCoinSlotImageRef.color = Color.white;
        //_MyCoinSlotImageRef.gameObject.GetComponent<CoinRotate>().YAxisSpeed = 1.0f;
    }

    // This is a different method than the one used to add a nameplate via CAPI. Buyer beware!
    public void AddInfoPlates()
    {
        // Instantiate the Object
        GameObject _newPlate = GameObject.Instantiate(InfoPlatesPrefab);
        _newPlate.transform.SetParent(ENVUI.transform);
        _newPlate.transform.SetAsFirstSibling();

        // Setup its variables
        _newPlate.GetComponent<OrbitalNameplate>().MyCamera = ENVCamera;
        _newPlate.GetComponent<OrbitalNameplate>().MyTarget = transform.parent.parent.gameObject;
        _newPlate.GetComponent<OrbitalNameplate>().FacingIndicator = ENVCamera.transform.GetChild(1).gameObject;

        // Customize the text
        //Debug.Log(_newPlate.transform.GetChild(0).name);
        _newPlate.transform.GetChild(0).GetComponent<Outline>().effectColor = transform.parent.parent.GetChild(3).GetComponent<SelectOrbitalObject>().OrbitalPathStartColor;
        _newPlate.transform.GetChild(0).GetComponent<Text>().text = "" + transform.parent.parent.name.ToUpper() + "\n[ " + MyType.ToString().ToUpper() + " ]";
    }

    public void DisableMyCache()
    {
        Debug.Log("Disabled " + gameObject.transform.parent.parent.name);
        gameObject.GetComponent<Button>().interactable = false;

        gameObject.GetComponent<AudioSource>().Play();
    }

    public void FoundCache()
    {
        switch (MyType)
        {
            case SmallWorldType.MAB:
                _MySceneController.MBO_Found++;
                Capi.set("Globals.MBOs Found", _MySceneController.MBO_Found);
                _MySceneController.TrophyCase.transform.Find("Toggle Cache Case").GetChild(3).GetChild(0).GetComponent<Text>().text = (_MySceneController.MBO_Found.ToString() + "/" + _MySceneController.MaxObjectsToFind.ToString());
                //_MySceneController.ToggleCUP = true;
                StartCoroutine(SetStudied());
                StartCoroutine(BlowUpCoinCache());
                MyReticle.MyTarget = ClearReticleTarget;

                CacheCase.Find("ToggleBoxMAB").transform.GetChild(0).GetComponent<SpinMeRightRoundBaby>().ToggleShownCoins();

                break;
            case SmallWorldType.OCO:


                break;
            case SmallWorldType.NEO:
                _MySceneController.NEO_Found++;
                Capi.set("Globals.NEOs Found", _MySceneController.NEO_Found);
                _MySceneController.TrophyCase.transform.Find("Toggle Cache Case").GetChild(2).GetChild(0).GetComponent<Text>().text = (_MySceneController.NEO_Found.ToString() + "/" + _MySceneController.MaxObjectsToFind.ToString());
                //_MySceneController.ToggleCUP = true;
                StartCoroutine(SetStudied());
                StartCoroutine(BlowUpCoinCache());
                MyReticle.MyTarget = ClearReticleTarget;

                CacheCase.Find("ToggleBoxNEO").transform.GetChild(0).GetComponent<SpinMeRightRoundBaby>().ToggleShownCoins();

                break;
            case SmallWorldType.KB:
                _MySceneController.KBO_Found++;
                Capi.set("Globals.KBOs Found", _MySceneController.KBO_Found);
                _MySceneController.TrophyCase.transform.Find("Toggle Cache Case").GetChild(4).GetChild(0).GetComponent<Text>().text = (_MySceneController.KBO_Found.ToString() + "/" + _MySceneController.MaxObjectsToFind.ToString());
                //_MySceneController.ToggleCUP = true;
                StartCoroutine(SetStudied());
                StartCoroutine(BlowUpCoinCache());
                MyReticle.MyTarget = ClearReticleTarget;

                CacheCase.Find("ToggleBoxKBO").transform.GetChild(0).GetComponent<SpinMeRightRoundBaby>().ToggleShownCoins();

                break;
            default:
                // obvs totes do nada
                break;
        }
    }
}
