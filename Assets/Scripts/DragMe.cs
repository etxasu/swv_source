using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragMe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public bool dragOnSurfaces = true;

    public bool CleanIcons;
    public bool AutoFlashZone;
    public bool ShowZone;
    public Camera MinimapCamera;
    public GameObject MySceneController;
    private SceneController _SceneController;
    public GameObject TestObjectPrefab;
    public GameObject ENVTestObjectPrefab;
    public GameObject ParentTo;
    public GameObject SolarSystemController;
    public GameObject DragSurface;
    public ZoneFlasher MyZoneFlasher;
    public OrbitalPathRenderer TestObjectOrbit;
    public string TestObjectPrefix;
    private Rect DragSurfaceRect;
    public bool ExposeCAPI;

    public GameObject[] ZoneBoundaryObjects;
    public GameObject[] TutorialZoneBoundaryObjects;
	
	private Dictionary<int,GameObject> m_DraggingIcons = new Dictionary<int, GameObject>();
	private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();

    private List<GameObject> _icons = new List<GameObject>();
    private List<GameObject> _TestObjects = new List<GameObject>();

    private int _myObjectList = 0;

    void Start()
    {
        _SceneController = MySceneController.GetComponent<SceneController>();
    }

	public void OnBeginDrag(PointerEventData eventData)
	{
        if(_icons.Count > 0)
        {
            CleanUpIcons();
        }

        //UnityEngine.Cursor.visible = false;

        var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        m_DraggingIcons[eventData.pointerId] = GameObject.Instantiate(TestObjectPrefab);

        m_DraggingIcons[eventData.pointerId].GetComponent<Image>().color = gameObject.GetComponent<Image>().color;
        m_DraggingIcons[eventData.pointerId].transform.SetParent (ParentTo.transform, false);
		m_DraggingIcons[eventData.pointerId].transform.SetAsLastSibling();
		
		var image = m_DraggingIcons[eventData.pointerId].GetComponent<Image>();
		// The icon will be under the cursor.
		// We want it to be ignored by the event system.
		var group = m_DraggingIcons[eventData.pointerId].AddComponent<CanvasGroup>();
		group.blocksRaycasts = false;

		image.sprite = GetComponent<Image>().sprite;
		//image.SetNativeSize();
		
		if (dragOnSurfaces)
			m_DraggingPlanes[eventData.pointerId] = transform as RectTransform;
		else
			m_DraggingPlanes[eventData.pointerId]  = canvas.transform as RectTransform;
		
		SetDraggedPosition(eventData);

        _icons.Add(m_DraggingIcons[eventData.pointerId]);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_DraggingIcons[eventData.pointerId] != null)
			SetDraggedPosition(eventData);
	}

	private void SetDraggedPosition(PointerEventData eventData)
	{
		if (dragOnSurfaces && eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
			m_DraggingPlanes[eventData.pointerId] = eventData.pointerEnter.transform as RectTransform;
		
		var rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();
		Vector3 globalMousePos;

		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position, eventData.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;
		}
	}

    public void OnEndDrag(PointerEventData eventData)
    {
        /*
		if (m_DraggingIcons[eventData.pointerId] != null)
			Destroy(m_DraggingIcons[eventData.pointerId]);
        */
        //m_DraggingIcons[eventData.pointerId] = null;

        UnityEngine.Cursor.visible = true;

        RepositionENVObjects();

        if (AutoFlashZone)
        { 
            MyZoneFlasher.ToggleZone = true;
        }
        
        TestObjectOrbit.ResizeTestObjectLine(ENVTestObjectPrefab.GetComponent<OrbitalMovement>().radius);
    }

    void Update()
    {
        if(ShowZone)
        {
            SortZoneBoundaries();
            MyZoneFlasher.ToggleZone = true;
            ShowZone = !ShowZone;
            Capi.set("UI.Minimap.FlashZone", ShowZone);
        }

        if(ExposeCAPI)
        {
            ExposeCapi();
            ExposeCAPI = false;
        }
    }

    void LateUpdate()
    {
        if(CleanIcons)
        {
            CleanUpIcons();
            CleanIcons = !CleanIcons;
        }
    }

    private void ExposeCapi()
    {
        Capi.expose<bool>(gameObject.name + ".AutoFlashZone", () => { return AutoFlashZone; }, (value) => { return AutoFlashZone = value; });
    }

    public void ReportIcons()
    {
        foreach(GameObject _GO in _icons)
        {
            //Debug.Log(_GO.GetComponent<RectTransform>().localPosition);
        }
    }


    public void RepositionENVObjects()
    {
        if (_icons.Count != 0)
        {
            float frustumHalfLength = (float)((Mathf.Tan(MinimapCamera.fieldOfView / 2) * MinimapCamera.transform.position.y) / -11.41);
            //Debug.Log(frustumHalfLength);
            DragSurfaceRect = DragSurface.GetComponent<RectTransform>().rect;

            foreach (GameObject _GO in _icons)
            {
                Vector3 RealPos = new Vector3((_GO.GetComponent<RectTransform>().localPosition.x / (DragSurfaceRect.width / 2)) * frustumHalfLength, 0.0f, (_GO.GetComponent<RectTransform>().localPosition.y / (DragSurfaceRect.height / 2)) * frustumHalfLength);
                //Debug.Log(RealPos.ToString());
                ENVTestObjectPrefab.transform.position = RealPos;
                OrbitalMovement OrbitalData = ENVTestObjectPrefab.GetComponent<OrbitalMovement>();
                OrbitalData.GetWorldSpeed();
                OrbitalData.InstantiateOrbitalPeriod();
                //ENVTestObjectPrefab.GetComponent<OrbitalMovement>().rotationSpeed = 20.0f / (5.57f * ENVTestObjectPrefab.GetComponent<OrbitalMovement>().radius);
                //Debug.Log(ENVTestObjectPrefab.GetComponent<OrbitalMovement>().rotationSpeed);
                OrbitalData.UpdateCapi();

                if (AutoFlashZone)
                {
                    SortZoneBoundaries();
                }
            }          
            CleanUpIcons();
            _icons.Clear();
        }
    }

    public void SortZoneBoundaries()
    {
        if (!_SceneController.TutorialMode)
        {
            ZoneBoundarySort(ZoneBoundaryObjects);
            SetFlasherZone();
        }
        else
        {
            //Debug.Log("Tutorial flash");
            ZoneBoundarySort(TutorialZoneBoundaryObjects);
            TutorialSetFlasherZone();
        }
    }

    public void TutorialSetFlasherZone()
    {
        // Find our test object. The array referenced in the first parameter is sorted prior to entering
        // this function. It better be.
        // JOS: 8/17/2016
        int _OrbitalIndex = Array.IndexOf(TutorialZoneBoundaryObjects, ENVTestObjectPrefab);

        float _zoneMidpoint = 0.0f;
        float _zoneWidth = 0.0f;
        GameObject _MySun = GameObject.Find("Sun");

        // Check against first and last first, we can't be either
        if (_OrbitalIndex != 0 && _OrbitalIndex != (TutorialZoneBoundaryObjects.Length - 1))
        {
            GameObject _InnerObject = TutorialZoneBoundaryObjects[_OrbitalIndex - 1];
            GameObject _OuterObject = TutorialZoneBoundaryObjects[_OrbitalIndex + 1];

            _zoneWidth = Vector3.Distance(_OuterObject.transform.position, _MySun.transform.position) - Vector3.Distance(_InnerObject.transform.position, _MySun.transform.position);
            _zoneMidpoint = _InnerObject.GetComponent<OrbitalMovement>().radius + (_zoneWidth / 2);

        }
        else if (_OrbitalIndex == 0) // In between sun and inner pole.
        {
            _zoneWidth = Vector3.Distance(TutorialZoneBoundaryObjects[1].transform.position, _MySun.transform.position);
            _zoneMidpoint = (_zoneWidth / 2);
        }
        else if (_OrbitalIndex == (TutorialZoneBoundaryObjects.Length - 1)) // Outer pole.
        {
            _zoneWidth = Vector3.Distance(TutorialZoneBoundaryObjects[2].transform.position, _MySun.transform.position);
            _zoneMidpoint = TutorialZoneBoundaryObjects[2].GetComponent<OrbitalMovement>().radius + (_zoneWidth / 2);
        }

        //_SceneController.AlterOrbitalZoneInfo((frustumHalfLength / 12.0f), OrbitalData.radius);
        _SceneController.AlterOrbitalZoneInfo(_zoneWidth, _zoneMidpoint, MyZoneFlasher.GetComponent<OrbitalPathRenderer>());
    }

    // In the future this likely needs to become a really ugly switch/case.
    // I don't have time to develop the correct answers for how this is supposed to be designed.
    // In the meantime, this is exactly what has been asked for.
    // JOS: 8/17/2016
    public void SetFlasherZone()
    {
        // Find our test object. The array referenced in the first parameter is sorted prior to entering
        // this function. It better be.
        // JOS: 8/17/2016
        int _OrbitalIndex = Array.IndexOf(ZoneBoundaryObjects, ENVTestObjectPrefab);

        float _zoneMidpoint = 0.0f;
        float _zoneWidth = 0.0f;
        GameObject _MySun = GameObject.Find("Sun");

        // Check against first and last first, we can't be either
        if(_OrbitalIndex != 0 && _OrbitalIndex != (ZoneBoundaryObjects.Length -1))
        {
            GameObject _InnerObject = ZoneBoundaryObjects[_OrbitalIndex - 1];
            GameObject _OuterObject = ZoneBoundaryObjects[_OrbitalIndex + 1];
            
            _zoneWidth = Vector3.Distance(_OuterObject.transform.position, _MySun.transform.position) - Vector3.Distance(_InnerObject.transform.position, _MySun.transform.position);
            _zoneMidpoint = _InnerObject.GetComponent<OrbitalMovement>().radius + (_zoneWidth / 2);
                        
        }
        else if(_OrbitalIndex == 0) // In between sun and Mercury
        {
            _zoneWidth = Vector3.Distance(ZoneBoundaryObjects[1].transform.position, _MySun.transform.position);
            _zoneMidpoint = (_zoneWidth / 2);
        }
        else if(_OrbitalIndex == (ZoneBoundaryObjects.Length - 1)) // Outside of Neptune. ZoneBoundaryObjects[6] should always be Neptune in this scenario
        {
            _zoneWidth = Vector3.Distance(ZoneBoundaryObjects[6].transform.position, _MySun.transform.position);
            _zoneMidpoint = ZoneBoundaryObjects[6].GetComponent<OrbitalMovement>().radius + (_zoneWidth / 2);
        }

        //_SceneController.AlterOrbitalZoneInfo((frustumHalfLength / 12.0f), OrbitalData.radius);
        _SceneController.AlterOrbitalZoneInfo(_zoneWidth, _zoneMidpoint, MyZoneFlasher.GetComponent<OrbitalPathRenderer>());
    }

    public void ZoneBoundarySort(GameObject[] _zones)
    {
        //Debug.Log(_zones);
        Array.Sort(_zones, delegate (GameObject _go1, GameObject _go2)
        {
            return _go1.GetComponent<OrbitalMovement>().radius.CompareTo(_go2.GetComponent<OrbitalMovement>().radius);
        });
    }

    public void CleanUpIcons()
    {
        foreach(var _GO in _icons)
        {
            Destroy(_GO);
        }

        _icons.Clear();           
    }
    public void CleanUpTestObjects()
    {
        foreach (var _GO in _TestObjects)
        {
            Destroy(_GO);
        }

        _TestObjects.Clear();
    }

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;
		
		var t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}
}
