using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[UxmlElement]
public partial class FlipElement : VisualElement
{
    private Image FlipSprite => this.Q<Image>("flip-sprite");

    public void SetFlipSprite(Sprite sprite)
    {
        FlipSprite.sprite = sprite;
    }

    public FlipElement() { }
}