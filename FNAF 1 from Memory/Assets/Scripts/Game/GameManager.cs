using UnityEngine;

// controls the game from main menu to gameplay

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    public int night;

    [SerializeField]
    AnimDifficulty[] defaultDifficulties;

    [SerializeField]
    AnimData[] animatronics;

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

    void InitGame()
    {

    }

    public void StartGame()
    {

    }

    public void SwitchCam()
    {

    }

    public void KillPlayer()
    {

    }
}
