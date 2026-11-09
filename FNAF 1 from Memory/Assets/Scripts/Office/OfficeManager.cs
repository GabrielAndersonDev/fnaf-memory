using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class OfficeManager : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction interactAction;
    InputAction lookAction;

    public event Action<bool> LDoorBtn;
    public event Action<bool> RDoorBtn;

    public event Action<bool> LLightBtn;
    public event Action<bool> RLightBtn;

    public bool isLDoorOpen;
    public bool isRDoorOpen;

    public bool isLLightOn;
    public bool isRLightOn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions.FindAction("Look");
        interactAction = playerInput.actions.FindAction("Interact");
    }

    void OnEnable()
    {
        lookAction.Enable();
        interactAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
        interactAction.Disable();
    }

    // mouse position set
    void Update()
    {
        MouseScreenCheck();
    }

    void MouseScreenCheck()
    {
        Vector3 mousePos = lookAction.ReadValue<Vector2>();
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
}
