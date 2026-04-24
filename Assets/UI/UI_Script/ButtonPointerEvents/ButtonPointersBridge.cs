using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ButtonPointersBridge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnPointerDownEvent;
    public event Action OnPointerUpEvent;

    public bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnPointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        OnPointerUpEvent?.Invoke();
    }

    private void OnDisable()
    {
        // Reset the flag if the button disappears 
        // so the player doesn't keep "attacking" or "moving"
        IsPressed = false;
    }
}
