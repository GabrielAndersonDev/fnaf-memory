using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public class SaveData
{
    public bool isNewSave;
    public int nightSaved;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Singleton { get; private set; }
    public bool isNewSave;

    public int nightSaved;

    public const string SaveFileName = "/savedata.json";

    InputAction delete;

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

    private void Start()
    {
        delete = GameManager.Singleton.inputSystem.cancel;
        delete.performed += DeleteSaveData;
    }

    private void OnDisable()
    {
        delete.performed -= DeleteSaveData;
    }

    private void SaveData()
    {
        isNewSave = false;

        string filePath = Application.persistentDataPath + SaveFileName;
        SaveData saveData = new()
        {
            isNewSave = false,
            nightSaved = nightSaved
        };
        string txt = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, txt);
    }

    private void LoadSaveData()
    {
        try
        {
            string filePath = Application.persistentDataPath + SaveFileName;
            string txt = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(txt);
            isNewSave = saveData.isNewSave;
            nightSaved = saveData.nightSaved;
        }
        catch
        {
            isNewSave = true;
        }
    }

    private void DeleteSaveData(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            try
            {
                string filePath = Application.persistentDataPath + SaveFileName;
                File.Delete(filePath);
            }
            catch
            {
                return;
            }
        }
    }

    public void SaveNight(bool isWon)
    {
        if (!isWon)
        {
            if (GameManager.Singleton.night == 1)
            {
                SaveData();
            }
            return;
        }

        if (GameManager.Singleton.night == 7)
        {
            AnimDifficulty dif = GameManager.Singleton.CustomDifficulty;

            if (dif.bonnie == 20
                && dif.chica == 20
                && dif.freddy == 20
                && dif.foxy == 20)
            {
                nightSaved = 7;
            }

            SaveData();
            return;
        }

        if (GameManager.Singleton.night > nightSaved)
        {
            nightSaved = GameManager.Singleton.night;

            SaveData();
        }
    }
}
