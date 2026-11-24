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

    public Dictionary<AnimatronicEnum, AnimData> AnimEnumToDataDictionary;
    public Dictionary<AnimatronicEnum, CamEnum> AnimLocations;

    private Coroutine BonnieRoutine;
    private Coroutine ChicaRoutine;
    private Coroutine FreddyRoutine;
    private Coroutine FoxyRoutine;
    private Coroutine GFreddyRoutine;

    public int foxyStage;
    public bool foxyAlreadySeen;

    public readonly static WaitForSeconds waitForSeconds5 = new(5f);
    public readonly static WaitForSeconds waitForSeconds7 = new(7f);

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

        PopulateAnimDictionaries();
        BeginAnimCoroutines();

        PowerManager.Singleton.PowerOutage += OnOutage;
        GameManager.Singleton.KillPlayer += OnNightEnd;
        GameManager.Singleton.NightWin += OnNightEnd;
        CamsManager.Singleton.OnCamSwitch += CheckCamState;
        CamsManager.Singleton.OnCamFlip += CheckFoxyCamFlip;
    }

    private void OnDisable()
    {
        PowerManager.Singleton.PowerOutage -= OnOutage;
        GameManager.Singleton.KillPlayer -= OnNightEnd;
        GameManager.Singleton.NightWin -= OnNightEnd;
        CamsManager.Singleton.OnCamSwitch -= CheckCamState;
        CamsManager.Singleton.OnCamFlip -= CheckFoxyCamFlip;
    }

    private void BeginAnimCoroutines()
    {
        foreach (AnimData anim in animatronics)
        {
            switch (anim.animatronic)
            {
                case AnimatronicEnum.Bonnie:
                    anim.InitAnim(animDifficulty.bonnie);
                    break;
                case AnimatronicEnum.Freddy:
                    anim.InitAnim(animDifficulty.freddy);
                    break;
                case AnimatronicEnum.Chica:
                    anim.InitAnim(animDifficulty.chica);
                    break;
                case AnimatronicEnum.Foxy:
                    anim.InitAnim(animDifficulty.foxy);
                    break;
                case AnimatronicEnum.GoldenFreddy:
                    break;
                default:
                    Debug.LogError("AIManager: invalid animatronic in AnimData.");
                    Debug.Break();
                    break;
            }

            AnimCoroutineSwitch(anim.animatronic, false);
        }
    }

    private void AnimCoroutineSwitch(AnimatronicEnum anim, bool isStall)
    {
        switch (anim)
        {
            case AnimatronicEnum.Bonnie:
                if (BonnieRoutine != null)
                {
                    StopCoroutine(BonnieRoutine);
                    BonnieRoutine = null;
                }
                BonnieRoutine ??= StartCoroutine(AnimEnumToDataDictionary[anim].AnimatronicRoutine(waitForSeconds5, isStall, waitForSeconds7));
                break;
            case AnimatronicEnum.Freddy:
                if (FreddyRoutine != null)
                {
                    StopCoroutine(FreddyRoutine);
                    FreddyRoutine = null;
                }
                FreddyRoutine ??= StartCoroutine(AnimEnumToDataDictionary[anim].AnimatronicRoutine(waitForSeconds5, isStall, waitForSeconds7));
                break;
            case AnimatronicEnum.Chica:
                if (ChicaRoutine != null)
                {
                    StopCoroutine(ChicaRoutine);
                    ChicaRoutine = null;
                }
                ChicaRoutine ??= StartCoroutine(AnimEnumToDataDictionary[anim].AnimatronicRoutine(waitForSeconds5, isStall, waitForSeconds7));
                break;
            case AnimatronicEnum.Foxy:
                if (FoxyRoutine != null)
                {
                    StopCoroutine(FoxyRoutine);
                    FoxyRoutine = null;
                }
                FoxyRoutine ??= StartCoroutine(AnimEnumToDataDictionary[anim].AnimatronicRoutine(waitForSeconds5, isStall, waitForSeconds7));
                break;
            case AnimatronicEnum.GoldenFreddy:
                break;
            default:
                Debug.LogError("AIManager: invalid animatronic in AnimData.");
                Debug.Break();
                break;
        }
    }

    void PopulateAnimDictionaries()
    {
        AnimLocations = new Dictionary<AnimatronicEnum, CamEnum>();
        AnimEnumToDataDictionary = new();

        foreach (var anim in animatronics)
        {
            AnimLocations.Add(anim.animatronic, anim.startLocation);
            AnimEnumToDataDictionary.Add(anim.animatronic, anim);
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

    private void CheckCamState(CamEnum cam, AnimatronicEnum[] anims, EventData[] events, int foxyStage)
    {
        if (cam == CamEnum.LHallway
            && anims.Contains(AnimatronicEnum.Foxy)
            && !foxyAlreadySeen)
        {
            SwapFoxyRoutine(true, true);
        }

        foreach (AnimatronicEnum anim in anims)
        {
            if (anim == AnimatronicEnum.Foxy) continue;

            AnimCoroutineSwitch(anim, true);
        }
    }

    public void SwapFoxyRoutine(bool isRunRoutine, bool isCamCheck)
    {
        Debug.Log("SwapFoxyRoutine ran: isRunRoutine: " + isRunRoutine + " isCamCheck: " + isCamCheck);

        if (!isRunRoutine 
            && isCamCheck)
        {
            if (FoxyRoutine != null) StopCoroutine(FoxyRoutine);
            return;
        }

        if (AnimEnumToDataDictionary.TryGetValue(AnimatronicEnum.Foxy, out AnimData anim))
        {
            if (isRunRoutine)
            {
                Foxy foxy = (Foxy)anim;

                WaitForSeconds waitTime = isCamCheck ? new(3f) : waitForSeconds5;

                if (isCamCheck) foxyAlreadySeen = true;

                if (FoxyRoutine != null) StopCoroutine(FoxyRoutine);
                FoxyRoutine = StartCoroutine(foxy.FoxyRunRoutine(waitTime));
            }
            else
            {
                AnimCoroutineSwitch(AnimatronicEnum.Foxy, false);
            }
        }
        else
        {
            Debug.LogError("AIManager: AnimEnumToData doesn't contain Foxy.");
            Debug.Break();
        }
    }

    // triggered whenever the cam flips up or down
    public void CheckFoxyCamFlip(bool isCamUp)
    {
        if (foxyStage != 4)
        {
            SwapFoxyRoutine(false, isCamUp);
        }
    }

    private void OnOutage()
    {
        StopAllCoroutines();

        if (FreddyRoutine != null) FreddyRoutine = null;

        FreddyRoutine ??= StartCoroutine(AnimEnumToDataDictionary[AnimatronicEnum.Freddy].FreddyOutage());
    }

    private void OnNightEnd()
    {
        StopAllCoroutines();
    }
}
