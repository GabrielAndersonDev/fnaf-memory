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

    private Coroutine FlickerRoutine;

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

        GameManager.Singleton.interact.performed += OnMouseClick;
        PowerManager.Singleton.PowerOutage += OnOutage;
        CamsManager.Singleton.OnCamFlip += CamListener;
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (isDisabled) return;

        if (context.ReadValue<float>() == 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(GameManager.Singleton.mousePos);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider == polygonCollider)
            {
                ButtonPressed();
            }
        }
    }

    private void CamListener(bool isUp)
    {
        isDisabled = isUp;

        if (isUp)
        {
            if (FlickerRoutine != null)
            {
                StopCoroutine(FlickerRoutine);
                FlickerRoutine = null;
            }

            spriteRenderer.sprite = null;
            if (isLight) OfficeManager.Singleton.DoorLightView(isLeft, false);
        }
        else
        {
            if (isPressed)
            {
                spriteRenderer.sprite = pressedSprite;
                FlickerRoutine ??= StartCoroutine(PressedAnim());
            }
            else
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }

    private void OnDisable()
    {
        GameManager.Singleton.interact.performed -= OnMouseClick;
        StopAllCoroutines();
        PowerManager.Singleton.PowerOutage -= OnOutage;
        CamsManager.Singleton.OnCamFlip -= CamListener;
    }

    public void ButtonPressed()
    {
        isPressed = !isPressed;

        OfficeManager.Singleton.ButtonPressed(isLeft, isLight, isPressed);

        if (isPressed)
        {
            spriteRenderer.sprite = pressedSprite;
            FlickerRoutine ??= StartCoroutine(PressedAnim());
        }
        else
        {
            if (FlickerRoutine != null)
            {
                StopCoroutine(FlickerRoutine);
                FlickerRoutine = null;
            }

            spriteRenderer.sprite = normalSprite;
            if (isLight) OfficeManager.Singleton.DoorLightView(isLeft, false);
        }
    }

    public IEnumerator PressedAnim()
    {
        while (true)
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
    }

    private void OnOutage()
    {
        enabled = false;
        GameManager.Singleton.interact.performed -= OnMouseClick;
        StopAllCoroutines();
        PowerManager.Singleton.PowerOutage -= OnOutage;
        CamsManager.Singleton.OnCamFlip -= CamListener;
    }
}
