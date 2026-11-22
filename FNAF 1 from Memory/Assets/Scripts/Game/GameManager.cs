using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

// controls the game from main menu to gameplay
public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    public int night;

    [SerializeField]
    AnimDifficulty[] defaultDifficulties;

    public event Action KillPlayer;
    public event Action NightWin;

    public bool isGameOver;

    public InputSystemUIInputModule inputSystem;

    public InputAction point;
    public Vector2 mousePos;

    public InputAction interact;
    public InputAction move;

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        inputSystem.actionsAsset.Enable();
        point = inputSystem.point;
        interact = inputSystem.leftClick;
        move = inputSystem.move;
    }

    private void OnDisable()
    {
        inputSystem.actionsAsset.Disable();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        mousePos = point.ReadValue<Vector2>();
    }

    public void StartGame()
    {
        // Function for swapping scenes to gaameplay and taking in night and difficulty info
        InitGame();
    }

    void InitGame()
    {
        if (night == 7)
        {
            Debug.LogError("Night seven info not set up yet.");
            Debug.Break();
        }
        else
        {
            AIManager.Singleton.animDifficulty = defaultDifficulties[night - 1];
        }

        CamsManager.Singleton.PopulateCamDictionary();
        AIManager.Singleton.InitAIManager();
    }

    public void OnKillPlayer()
    {
        Debug.LogError("Player was killed, uniplemented");
        isGameOver = true;
        KillPlayer.Invoke();
    }

    public void OnNightWin()
    {
        isGameOver = true;
        NightWin.Invoke();
    }
}
