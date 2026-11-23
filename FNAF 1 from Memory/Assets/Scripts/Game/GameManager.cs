using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

// controls the game from main menu to gameplay
public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    public int night;

    [SerializeField]
    public AnimDifficulty[] defaultDifficulties;
    public AnimDifficulty CustomDifficulty { get; private set; }

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

    private void Update()
    {
        mousePos = point.ReadValue<Vector2>();
    }

    public void StartGame(int night)
    {
        this.night = night;

        // Function for swapping scenes to gaameplay and taking in night and difficulty info
        if (SaveManager.Singleton.isNewSave)
        {
            // load newspaper
            Debug.LogWarning("Loads the newspaper here");
        }

        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void StartCustomNight(int bonnie, int freddy, int chica, int foxy)
    {
        if (bonnie == 1
            && freddy == 9
            && chica == 8
            && foxy == 7)
        {
            GoldenFreddyKill();
            return;
        }

        AnimDifficulty customDiff = new()
        {
            night = 7,
            bonnie = bonnie,
            freddy = freddy,
            chica = chica,
            foxy = foxy
        };

        CustomDifficulty = customDiff;

        StartGame(7);
    }

    public void OnKillPlayer()
    {
        Debug.LogError("Player was killed, uniplemented");
        isGameOver = true;
        KillPlayer.Invoke();

        EndGame(false);
    }

    public void OnNightWin()
    {
        isGameOver = true;
        NightWin.Invoke();

        EndGame(true);
    }

    private void EndGame(bool isNightWon)
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        if (isNightWon)
        {
            SaveManager.Singleton.SaveNightWin();
        }
    }

    public void GoldenFreddyKill()
    {
        Debug.LogError("GameManager: Golden Freddy kill not implemented");
        Debug.Break();
    }
}
