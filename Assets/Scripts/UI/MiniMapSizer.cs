using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMapSizer : MonoBehaviour
{
    public bool MiniMapExpanded = false;
    public Vector3 ExpandedPosition;
    public Vector2 ExpandedSize;

    public GameObject ZoomMask;
    public GameObject MinimapImage;

    private RectTransform _OriginalRect;
    private Vector3 _OriginalRectPosition;
    private Vector2 _OriginalSize;

    private Vector2 _ExpandedInsetSize;
    private Vector2 _OriginalInsetSize;

    public GameObject DragControllerObject;
    public string ExpandedLabel;
    public string CompressedLabel;

    // Use this for initialization
    void Start ()
    {
        _OriginalRect = transform.parent.GetComponent<RectTransform>();
        _OriginalRectPosition = _OriginalRect.localPosition;
        _OriginalSize = _OriginalRect.rect.size;

        _OriginalInsetSize = ZoomMask.GetComponent<RectTransform>().rect.size;

        gameObject.transform.GetChild(1).GetComponent<Text>().text = CompressedLabel;

        _ExpandedInsetSize.x = ExpandedSize.x - 4;
        _ExpandedInsetSize.y = ExpandedSize.y - 4;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ResizePanel()
    {
        if(MiniMapExpanded)
        {
            _OriginalRect.localPosition = _OriginalRectPosition;
            _OriginalRect.sizeDelta = _OriginalSize;

            ZoomMask.GetComponent<RectTransform>().sizeDelta = _OriginalInsetSize;
            MinimapImage.GetComponent<RectTransform>().sizeDelta = _OriginalInsetSize;

            gameObject.transform.GetChild(1).GetComponent<Text>().text = CompressedLabel;        

            MiniMapExpanded = !MiniMapExpanded;
        }
        else
        {
            _OriginalRect.localPosition = ExpandedPosition;
            _OriginalRect.sizeDelta = ExpandedSize;

            ZoomMask.GetComponent<RectTransform>().sizeDelta = _ExpandedInsetSize;
            MinimapImage.GetComponent<RectTransform>().sizeDelta = _ExpandedInsetSize;

            gameObject.transform.GetChild(1).GetComponent<Text>().text = ExpandedLabel;

            MiniMapExpanded = !MiniMapExpanded;
        }

        DragControllerObject.GetComponent<DragMe>().CleanUpIcons();
    }
}
