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

    public DoorController leftDoor;
    public DoorController rightDoor;

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
        //interact.performed += ClickCheckUwU;

        AssignButtons();
    }

    // temporary for btn testing
    //private void ClickCheckUwU(InputAction.CallbackContext context)
    //{
    //    if (context.ReadValue<float>() == 0f)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(GameManager.Singleton.mousePos);
    //        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

    //        if (hit.collider != null)
    //        {
    //            GameObject clickedObj = hit.collider.gameObject;
    //            Debug.Log(clickedObj.name);
    //        }
    //        else
    //        {
    //            Debug.Log("Hitting object without collider");
    //        }
    //    }
    //}

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
        BtnPressed.Invoke(isLeft, isLight, isPressed);
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
        //interact.performed -= ClickCheckUwU;
    }

    // mouse position set
    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        CalcSpriteWidth();
        MouseScreenCheck();
    }

    void MouseScreenCheck()
    {
        if (CamsManager.Singleton.inCams)
        {
            return;
        }

        Vector2 mousePos = GameManager.Singleton.mousePos;

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

    public void DoorLightView(bool isLeft, bool isOn)
    {
        if (isLeft)
        {
            leftDoor.SetSprites(true, isOn);
        }
        else
        {
            rightDoor.SetSprites(true, isOn);
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

        Debug.Log(waitTime);

        yield return waitTime;
    }
}
