using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AnimatronicEnum
{
    Invalid = -2,
    None = -1,
    First,
    Bonnie = First,
    Freddy,
    Chica,
    Foxy,
    GoldenFreddy,
    Max
}

public class AIManager : MonoBehaviour
{
    public static AIManager Singleton { get; private set; }

    public AnimDifficulty animDifficulty;

    public AnimData[] animatronics;

    public Dictionary<AnimatronicEnum, CamEnum> AnimLocations;

    private Coroutine BonnieRoutine;
    private Coroutine FreddyRoutine;
    private Coroutine ChicaRoutine;
    private Coroutine FoxyRoutine;
    private Coroutine FoxyRunRoutine;
    private Coroutine GFreddyRoutine;

    public int foxyStage;

    public event Action<bool, bool> AnimAtDoor;

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
        InitAIManager();
    }

    public void InitAIManager()
    {
        if (GameManager.Singleton.night == 7)
        {
            animDifficulty = GameManager.Singleton.CustomDifficulty;
        }
        else
        {
            animDifficulty = GameManager.Singleton.defaultDifficulties[GameManager.Singleton.night - 1];
        }

        PopulateAnimLocations();
        BeginAnimCoroutines();

        PowerManager.Singleton.PowerOutage += OnOutage;
        GameManager.Singleton.KillPlayer += OnNightEnd;
        GameManager.Singleton.NightWin += OnNightEnd;
    }

    private void OnDisable()
    {
        PowerManager.Singleton.PowerOutage -= OnOutage;
        GameManager.Singleton.KillPlayer -= OnNightEnd;
        GameManager.Singleton.NightWin -= OnNightEnd;
    }

    private void BeginAnimCoroutines()
    {
        foreach (AnimData anim in animatronics)
        {
            switch (anim.animatronic)
            {
                case AnimatronicEnum.Bonnie:
                    anim.InitAnim(animDifficulty.bonnie);
                    BonnieRoutine ??= StartCoroutine(anim.AnimatronicRoutine());
                    break;
                case AnimatronicEnum.Freddy:
                    anim.InitAnim(animDifficulty.freddy);
                    FreddyRoutine ??= StartCoroutine(anim.AnimatronicRoutine());
                    break;
                case AnimatronicEnum.Chica:
                    anim.InitAnim(animDifficulty.chica);
                    ChicaRoutine ??= StartCoroutine(anim.AnimatronicRoutine());
                    break;
                case AnimatronicEnum.Foxy:
                    anim.InitAnim(animDifficulty.foxy);
                    FoxyRoutine ??= StartCoroutine(anim.AnimatronicRoutine());
                    break;
                case AnimatronicEnum.GoldenFreddy:
                    break;
                default:
                    Debug.LogError("AIManager: invalid animatronic in AnimData.");
                    Debug.Break();
                    break;
            }
        }
    }

    void PopulateAnimLocations()
    {
        AnimLocations = new Dictionary<AnimatronicEnum, CamEnum>();

        foreach (var anim in animatronics)
        {
            AnimLocations.Add(anim.animatronic, anim.startLocation);
        }
    }

    public void UpdateAnimLocation(AnimatronicEnum anim, CamEnum newLocation)
    {
        if (AnimLocations.ContainsKey(anim))
        {
            CamEnum prevLocation = AnimLocations[anim];
            AnimLocations[anim] = newLocation;

            if (newLocation == CamEnum.LDoor
                || prevLocation == CamEnum.LDoor)
            {
                AnimAtDoor.Invoke(true, newLocation == CamEnum.LDoor);
            }
            else if (newLocation == CamEnum.RDoor
                || prevLocation == CamEnum.RDoor)
            {
                AnimAtDoor.Invoke(true, newLocation == CamEnum.RDoor);
            }

            if (CamsManager.Singleton.inCams)
            {
                CamEnum currentCam = CamsManager.Singleton.currentCam;

                if (currentCam == newLocation
                    || currentCam == prevLocation)
                {
                    // play shitrate effect
                    CamsManager.Singleton.SwitchCam(currentCam);
                }
            }

            Debug.Log("Animatronic " + anim + " is now at " + newLocation);
        }
        else
        {
            Debug.LogError($"AIManager: AnimLocations does not contain {anim}.");
            Debug.Break();
        }
    }

    public void RunFoxyCoroutine(IEnumerator routine)
    {
        FoxyRunRoutine ??= StartCoroutine(routine);
    }

    private void OnOutage()
    {
        StopAllCoroutines();

        foreach (AnimData anim in animatronics)
        {
            if (anim.animatronic == AnimatronicEnum.Freddy)
            {
                FreddyRoutine ??= StartCoroutine(anim.FreddyOutage());
            }
        }
    }

    private void OnNightEnd()
    {
        StopAllCoroutines();
    }
}
