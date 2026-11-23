using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Singleton { get; private set; }
    public bool isNewSave;

    public int nightSaved;
    public bool nightFiveCompleted;
    public bool nightSixCompleted;
    public bool maxModeCompleted;

    private void Awake()
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

        LoadSaveData();
    }

    private void SaveData()
    {
        Debug.LogWarning("SaveManager: no data to save, not implemented");
    }

    private void LoadSaveData()
    {
        Debug.LogWarning("SaveManager: no save data to load, not implemented");
    }

    private void DeleteSaveData()
    {
        Debug.LogWarning("SaveManager: no save data to load, not implemented");
    }

    public void SaveNightWin()
    {
        if (GameManager.Singleton.night == 7)
        {
            AnimDifficulty dif = GameManager.Singleton.CustomDifficulty;

            if (dif.bonnie == 20
                && dif.freddy == 20
                && dif.chica == 20
                && dif.foxy == 20)
            {
                nightSaved = 7;
                maxModeCompleted = true;
            }

            SaveData();
            return;
        }

        if (GameManager.Singleton.night > nightSaved)
        {
            nightSaved = GameManager.Singleton.night;

            if (nightSaved == 5) nightFiveCompleted = true;
            if (nightSaved == 6) nightSixCompleted = true;

            SaveData();
        }
    }
}
