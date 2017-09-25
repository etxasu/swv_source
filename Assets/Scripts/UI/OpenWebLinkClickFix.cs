using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class OpenWebLinkClickFix : MonoBehaviour, IPointerDownHandler {

    [Serializable]
    public class ButtonPressEvent : UnityEvent { }

    public ButtonPressEvent OnPress = new ButtonPressEvent();
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress.Invoke();
    }
}
