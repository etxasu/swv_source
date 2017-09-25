using UnityEngine;
using UnityEngine.EventSystems;

public class RenderTextureRaycaster : MonoBehaviour, IPointerDownHandler
{
    public Camera portalExit;

    void Start()
    {
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // This 11.11 is a magical constant.
        // I do not care why or how it works at this point.
        // It works. That is enough.
        // JOS: 7/21/2016
        //Debug.Log(((Mathf.Tan(portalExit.fieldOfView/2) * portalExit.transform.position.y)/-11.11).ToString());
        //Debug.Log("OnPointerDown called by " + gameObject.name);
    }
}