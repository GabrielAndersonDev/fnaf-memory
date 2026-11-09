using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[UxmlElement]
public partial class FlipElement : VisualElement, IPointerEnterHandler, IPointerExitHandler
{
    bool isHovering;
    public event Action TriggerCamFlip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovering)
        {
            return;
        }
        else
        {
            TriggerCamFlip?.Invoke();
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public FlipElement() { }
}