using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMe : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image containerImage;
	public Image receivingImage;
	private Color normalColor;
	public Color highlightColor = Color.yellow;
    public Camera CastingCamera;
	
	public void OnEnable ()
	{
		if (containerImage != null)
			normalColor = containerImage.color;
	}
	
	public void OnDrop(PointerEventData data)
	{
		containerImage.color = normalColor;
		
		if (receivingImage == null)
			return;
		
		//Sprite dropSprite = GetDropSprite (data);


        DebugPoint(data);
        /*
        if (dropSprite != null)
			receivingImage.overrideSprite = dropSprite;
        */
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if (containerImage == null)
			return;

		Sprite dropSprite = GetDropSprite (data);
		if (dropSprite != null)
			containerImage.color = highlightColor;
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (containerImage == null)
			return;
		
		containerImage.color = normalColor;
	}
	
	private Sprite GetDropSprite(PointerEventData data)
	{
		var originalObj = data.pointerDrag;
		if (originalObj == null)
			return null;
		
		var dragMe = originalObj.GetComponent<DragMe>();
		if (dragMe == null)
			return null;
		
		var srcImage = originalObj.GetComponent<Image>();
		if (srcImage == null)
			return null;
		
		return srcImage.sprite;
	}

    public void CastToENV()
    {
        var miniMapRect = gameObject.GetComponent<RectTransform>().rect;
        var screenRect = new Rect(
            gameObject.transform.position.x,
            gameObject.transform.position.y,
            miniMapRect.width, miniMapRect.height);

        var mousePos = Input.mousePosition;
        mousePos.y -= screenRect.y;
        mousePos.x -= screenRect.x;
        
        /*
        var camPos = new Vector3(
            mousePos.x * (MapWidth / screenRect.width),
            mousePos.y * (MapHeight / screenRect.height),
            Camera.main.transform.position.z);
        CastingCamera.transform.position = camPos;
        */

    }

    public void DebugPoint(PointerEventData ped)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), ped.position, ped.pressEventCamera, out localCursor))
            return;

        //Debug.Log("LocalCursor:" + localCursor);
    }
}
