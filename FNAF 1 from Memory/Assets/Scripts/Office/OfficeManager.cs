using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Singleton { get; private set; }

    [SerializeField]
    BtnController[] buttons;

    public event Action<bool, bool, bool> BtnPressed;

    public bool isLDoorClosed;
    public bool isRDoorClosed;

    public bool isLLightOn;
    public bool isRLightOn;

    public bool isLDoorBtnDisabled;
    public bool isRDoorBtnDisabled;

    [SerializeField]
    Sprite lightNormalBtn;
    [SerializeField]
    Sprite lightPressedBtn;
    [SerializeField]
    Sprite doorNormalBtn;
    [SerializeField]
    Sprite doorPressedBtn;

    [SerializeField]
    SpriteRenderer officeSprite;
    [SerializeField]
    Sprite outageOffice;

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    float baseSpeed;

    float maxMoveDistanceX;

    InputAction point;
    InputAction interact;
    InputAction look;

    Vector2 mousePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        point = GameManager.Singleton.inputSystem.point;
        interact = GameManager.Singleton.inputSystem.leftClick;
        look = GameManager.Singleton.inputSystem.move;

        PowerManager.Singleton.PowerOutage += OnOutage;

        AssignButtons();
    }

    private void AssignButtons()
    {
        foreach (BtnController btn in buttons)
        {
            Sprite pressed;
            Sprite normal;

            pressed = btn.isLight ? lightPressedBtn : doorPressedBtn;
            normal = btn.isLight ? lightNormalBtn : doorNormalBtn;

            btn.InitBtn(pressed, normal);
        }
    }

    public void ButtonPressed(bool isLeft, bool isLight, bool isPressed)
    {
        if (isLeft)
        {
            if (isLight)
            {
                isLLightOn = isPressed;
            }
            else
            {
                isLDoorClosed = isPressed;
            }
        }
        else
        {
            if (isLight)
            {
                isRLightOn = isPressed;
            }
            else
            {
                isRDoorClosed = isPressed;
            }
        }
    }

    private void CalcSpriteWidth()
    {
        float spriteWidth = officeSprite.bounds.size.x;

        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;

        maxMoveDistanceX = Mathf.Abs((spriteWidth - camWidth) / 2);
    }

    private void OnDisable()
    {
        PowerManager.Singleton.PowerOutage -= OnOutage;
    }

    // mouse position set
    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        mousePos = point.ReadValue<Vector2>();
        CalcSpriteWidth();
        MouseScreenCheck();
    }

    void MouseScreenCheck()
    {
        if (CamsManager.Singleton.inCams)
        {
            return;
        }

        float minLeftPos = (float)(Screen.width * 0.2);
        float minRightPos = (float)Screen.width - minLeftPos;

        float transformSpeed;


        // check left
        if (mousePos.x <= minLeftPos
            && officeSprite.gameObject.transform.position.x < maxMoveDistanceX)
        {
            transformSpeed = baseSpeed * Mathf.Abs(mousePos.x - minLeftPos);
            officeSprite.gameObject.transform.Translate(transformSpeed * Time.deltaTime, 0, 0);
        }

        // check right
        if (mousePos.x >= minRightPos
            && officeSprite.gameObject.transform.position.x > -maxMoveDistanceX)
        {
            transformSpeed = baseSpeed * (mousePos.x - minRightPos);
            officeSprite.gameObject.transform.Translate(-transformSpeed * Time.deltaTime, 0, 0);
        }

        if (officeSprite.gameObject.transform.position.x > maxMoveDistanceX)
        {
            officeSprite.gameObject.transform.position = new Vector3(maxMoveDistanceX, 0, 0);
        }

        if (officeSprite.gameObject.transform.position.x < -maxMoveDistanceX)
        {
            officeSprite.gameObject.transform.position = new Vector3(-maxMoveDistanceX, 0, 0);
        }
    }



    private void OnOutage()
    {
        // Play outage animation (opening doors)

        officeSprite.sprite = outageOffice;
    }

    public IEnumerator FreddyOutageAnim(float time, int freddyStage)
    {
        WaitForSeconds waitTime = new(time);

        Debug.LogError("OfficeManager: FreddyOutageAnim is still unimplemented");

        yield return waitTime;
    }
}
