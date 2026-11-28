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
    Chica,
    Freddy,
    Foxy,
    GoldenFreddy,
    Max
}

public class AIManager : MonoBehaviour
{
    public static AIManager Singleton { get; private set; }

    public AnimDifficulty animDifficulty;
    public AnimationCurve difficultyCurve;

    public Dictionary<AnimatronicEnum, AnimData> AnimEnumToDataDictionary;
    public Dictionary<AnimatronicEnum, CamEnum> AnimLocations;

    private Coroutine[] AnimCoroutines;

    public int foxyStage;
    public bool foxyAlreadySeen;
    public bool isGFreddyEvent;

    public readonly static WaitForSeconds waitForSeconds5 = new(5f);
    public readonly static WaitForSeconds waitForSeconds7 = new(7f);

    public event Action<bool, bool> AnimAtDoor;
    public event Action<bool> GFreddyInOffice;

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

        isGFreddyEvent = false;

        PowerManager.Singleton.PowerOutage += OnOutage;
        GameManager.Singleton.KillPlayer += OnNightEnd;
        GameManager.Singleton.NightWin += OnNightEnd;
        CamsManager.Singleton.OnCamSwitch += CheckCamState;
        CamsManager.Singleton.OnCamFlip += CheckCamFlip;
    }

    private void OnDisable()
    {
        PowerManager.Singleton.PowerOutage -= OnOutage;
        GameManager.Singleton.KillPlayer -= OnNightEnd;
        GameManager.Singleton.NightWin -= OnNightEnd;
        CamsManager.Singleton.OnCamSwitch -= CheckCamState;
        CamsManager.Singleton.OnCamFlip -= CheckCamFlip;
    }

    private void BeginAnimCoroutines()
    {
        AnimCoroutines = new Coroutine[GameManager.Singleton.Anims.Length];

        foreach (AnimData anim in GameManager.Singleton.Anims)
        {
            if (anim.animatronic == AnimatronicEnum.GoldenFreddy)
            {
                continue;
            }

            try
            {
                int difficulty = animDifficulty.difficulty[(int)anim.animatronic];
                Debug.Log("Difficulty: " + difficulty + ". Anim: " + anim.animName);
                anim.InitAnim(difficulty);
                AnimCoroutineSwitch(anim.animatronic, false);
            }
            catch
            {
                Debug.LogError("AIManager: Failed to initialize anim coroutines.");
                Debug.Break();
            }
        }
    }

    private void AnimCoroutineSwitch(AnimatronicEnum anim, bool isStall)
    {
        try
        {
            if (AnimEnumToDataDictionary[anim].isStalled)
            {
                return;
            }

            if (AnimCoroutines[(int)anim] != null)
            {
                StopCoroutine(AnimCoroutines[(int)anim]);
                AnimCoroutines[(int)anim] = null;
            }

            AnimCoroutines[(int)anim] ??= StartCoroutine(AnimEnumToDataDictionary[anim].AnimatronicRoutine(waitForSeconds5, isStall, waitForSeconds7));
        }
        catch
        {
            Debug.LogError("Failed to restart AnimCoroutine");
            Debug.Break();
        }
    }

    void PopulateAnimDictionaries()
    {
        AnimLocations = new Dictionary<AnimatronicEnum, CamEnum>();
        AnimEnumToDataDictionary = new();

        foreach (var anim in GameManager.Singleton.Anims)
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

        if (isGFreddyEvent
            && cam != CamEnum.Janitor)
        {
            isGFreddyEvent = false;
        }

        foreach (AnimatronicEnum anim in anims)
        {
            if (anim == AnimatronicEnum.Foxy
                || anim == AnimatronicEnum.GoldenFreddy) continue;

            AnimCoroutineSwitch(anim, true);
        }
    }

    public void SwapFoxyRoutine(bool isRunRoutine, bool isCamUp)
    {
        if (!isRunRoutine
            && isCamUp)
        {
            if (AnimCoroutines[(int)AnimatronicEnum.Foxy] != null)
            {
                StopCoroutine(AnimCoroutines[(int)AnimatronicEnum.Foxy]);
                AnimCoroutines[(int)AnimatronicEnum.Foxy] = null;
                return;
            }
        }

        if (AnimEnumToDataDictionary.TryGetValue(AnimatronicEnum.Foxy, out AnimData anim))
        {
            if (isRunRoutine)
            {
                Foxy foxy = (Foxy)anim;

                WaitForSeconds waitTime = isCamUp ? new(3f) : waitForSeconds5;

                if (isCamUp && CamsManager.Singleton.currentCam == CamEnum.LHallway) foxyAlreadySeen = true;

                if (AnimCoroutines[(int)AnimatronicEnum.Foxy] != null)
                {
                    StopCoroutine(AnimCoroutines[(int)AnimatronicEnum.Foxy]);
                    AnimCoroutines[(int)AnimatronicEnum.Foxy] = null;
                }

                AnimCoroutines[(int)AnimatronicEnum.Foxy] ??= StartCoroutine(foxy.FoxyRunRoutine(waitTime));
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
    public void CheckCamFlip(bool isCamUp)
    {
        if (foxyStage != 4)
        {
            SwapFoxyRoutine(false, isCamUp);
        }

        if (isGFreddyEvent
            && !isCamUp)
        {
            AnimCoroutineSwitch(AnimatronicEnum.GoldenFreddy, false);
            GFreddyInOffice.Invoke(true);
            isGFreddyEvent = false;
        }
        else if (AnimCoroutines[(int)AnimatronicEnum.GoldenFreddy] != null
            && isCamUp)
        {
            GFreddyOff();
        }
    }

    public void GFreddyOff()
    {
        if (AnimCoroutines[(int)AnimatronicEnum.GoldenFreddy] != null)
        {
            StopCoroutine(AnimCoroutines[(int)AnimatronicEnum.GoldenFreddy]);
            AnimCoroutines[(int)AnimatronicEnum.GoldenFreddy] = null;
        }
        else
        {
            Debug.LogError("GFreddyOff triggered while coroutine is null.");
            Debug.Break();
        }

        isGFreddyEvent = false;
        GFreddyInOffice.Invoke(false);
        OfficeManager.Singleton.GFreddyToggle(false);
        UpdateAnimLocation(AnimatronicEnum.GoldenFreddy, CamEnum.Janitor);
    }

    private void OnOutage()
    {
        StopAllCoroutines();

        if (AnimCoroutines[(int)AnimatronicEnum.Freddy] != null) AnimCoroutines[(int)AnimatronicEnum.Freddy] = null;

        AnimCoroutines[(int)AnimatronicEnum.Freddy] ??= StartCoroutine(AnimEnumToDataDictionary[AnimatronicEnum.Freddy].FreddyOutage());
    }

    private void OnNightEnd()
    {
        StopAllCoroutines();
    }
}
