using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Singleton { get; private set; }
    public PlayerInput playerInput;

    public event Action<bool> LDoorBtn;
    public event Action<bool> RDoorBtn;

    public event Action<bool> LLightBtn;
    public event Action<bool> RLightBtn;

    public bool isLDoorClosed;
    public bool isRDoorClosed;

    public bool isLLightOn;
    public bool isRLightOn;

    public bool isLDoorBtnDisabled;
    public bool isRDoorBtnDisabled;

    [SerializeField]
    SpriteRenderer officeSprite;
    [SerializeField]
    Sprite outageOffice;

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
        playerInput = GetComponent<PlayerInput>();
        PowerManager.Singleton.PowerOutage += OnOutage;
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

        Vector2 mousePos = Mouse.current.position.ReadValue();
    }

    void MouseScreenCheck(Vector2 mousePos)
    {
        float baseSpeed = 1;

        float screenDif = (float)(Screen.width * 0.2);

        float minLeftPos = screenDif;
        float minRightPos = (float)Screen.width - screenDif;

        // check left
        if (mousePos.x <= minLeftPos)
        {   
            // move sprite/cam POV here
        }

        // check right
        if (mousePos.x >= minRightPos)
        {
            //move sprite/cam POV here
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
