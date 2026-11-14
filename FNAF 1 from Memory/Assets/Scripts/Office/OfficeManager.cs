using System;
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

    public bool isLDoorOpen;
    public bool isRDoorOpen;

    public bool isLLightOn;
    public bool isRLightOn;

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

        playerInput = GetComponent<PlayerInput>();
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
}
