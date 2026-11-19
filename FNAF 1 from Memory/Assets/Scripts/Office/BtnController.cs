using System;
using UnityEngine;
using UnityEngine.UI;

public class BtnController : MonoBehaviour
{
    public bool isLeft;
    public bool isLight;
    public Button button;
    public SpriteRenderer sprite;

    private Sprite pressedSprite;
    private Sprite normalSprite;

    public bool isPressed;
    public bool isDisabled;

    public void InitBtn(Sprite pressed, Sprite normal)
    {
        pressedSprite = pressed;
        normalSprite = normal;

        button.onClick.AddListener(() => ButtonPressed());
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void ButtonPressed()
    {
        isPressed = !isPressed;

        OfficeManager.Singleton.ButtonPressed(isLeft, isLight, isPressed);

        if (isPressed)
        {
            sprite.sprite = pressedSprite;
            PressedAnim();
        }
        else
        {
            sprite.sprite = normalSprite;
            // cancel pressed anim
        }
    }

    public void PressedAnim()
    {
        Debug.LogWarning("PressedAnim in Btn is unimplemented");
    }
}
