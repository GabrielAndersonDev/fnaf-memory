using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;


[RequireComponent(typeof(PolygonCollider2D))]
public class BtnController : MonoBehaviour
{
    public bool isLeft;
    public bool isLight;
    public SpriteRenderer spriteRenderer;

    private Sprite pressedSprite;
    private Sprite normalSprite;

    public bool isPressed;
    public bool isDisabled;

    PolygonCollider2D polygonCollider;

    public void InitBtn(Sprite pressed, Sprite normal)
    {
        pressedSprite = pressed;
        normalSprite = normal;

        polygonCollider = GetComponent<PolygonCollider2D>();

        if (isLeft)
        {
            spriteRenderer.sprite = normalSprite;
        }
        else
        {
            // reflect it when i remember how to
            spriteRenderer.flipY = true;
            spriteRenderer.sprite = normalSprite;
        }

        Debug.Log("FREDDY FAZBEAR DAMN");

        GameManager.Singleton.interact.performed += OnMouseClick;
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(GameManager.Singleton.mousePos);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider == polygonCollider)
            {
                ButtonPressed();
                DebugListener();
            }
        }
    }

    private void OnDisable()
    {
        GameManager.Singleton.interact.performed -= OnMouseClick;
        StopAllCoroutines();
    }

    public void DebugListener()
    {
        Debug.Log("Button : " + (isLeft ? "left" : "right") + (isLight ? " light" : " door") + " " + (isPressed ? " is active." : " is off."));
    }

    public void ButtonPressed()
    {
        isPressed = !isPressed;

        OfficeManager.Singleton.ButtonPressed(isLeft, isLight, isPressed);

        if (isPressed)
        {
            spriteRenderer.sprite = pressedSprite;
            StartCoroutine(PressedAnim());
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public IEnumerator PressedAnim()
    {
        while (isPressed)
        {
            WaitForSeconds waitTime = new(UnityEngine.Random.Range(0.3f, 1f));
            yield return waitTime;

            spriteRenderer.sprite = normalSprite;

            if (isLight) OfficeManager.Singleton.DoorLightView(isLeft, false);

            WaitForSeconds pause = new(UnityEngine.Random.Range(0.05f, 0.2f));
            yield return pause;

            spriteRenderer.sprite = pressedSprite;
            if (isLight) OfficeManager.Singleton.DoorLightView(isLeft, true);
        }

        spriteRenderer.sprite = normalSprite;
        if (isLight) OfficeManager.Singleton.DoorLightView(isLeft, false);

        yield return null;
    }
}
