using System;
using System.Collections.Generic;
using UnityEngine;

public enum CamEnum 
{
    Invalid = -2,
    None = -1,
    First,
    Bathroom = First,
    FourB,
    Janitor,
    Kitchen,
    LHallway,
    RHallway,
    PartsService,
    Party,
    PirateCove,
    Stage,
    Office,
    LDoor,
    RDoor,
    Max
}

public class CamsManager : MonoBehaviour
{
    public static CamsManager Singleton {  get; private set; }
    public Dictionary<CamEnum, CamData> EnumCamsDictionary;

    public CamData[] camDataArr;

    public CamEnum currentCam;
    public bool inCams;
    private bool isCamsDisabled;

    public event Action<bool> OnCamFlip;

    // event will probably change when i get those working, int is foxy stages
    public event Action<CamEnum, AnimatronicEnum[], EventData[], int> OnCamSwitch;

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
        }

        isCamsDisabled = false;
    }

    private void Start()
    {
        PowerManager.Singleton.PowerOutage += OnOutage;
    }

    private void OnDisable()
    {
        PowerManager.Singleton.PowerOutage -= OnOutage;
    }

    public void PopulateCamDictionary()
    {
        if (camDataArr != null)
        {
            if (camDataArr.Length != 10)
            {
                Debug.LogError("CamsManager: missing cams in camDataArr.");
                Debug.Break();
            }

            EnumCamsDictionary = new Dictionary<CamEnum, CamData>();

            foreach (CamData data in camDataArr)
            {
                CamEnum camEnum = (CamEnum)data.camID;

                if (EnumCamsDictionary.TryAdd(camEnum, data))
                {
                    continue;
                }
                else
                {
                    Debug.LogError("CamsManager: Failed to add " + data.name + " to EnumCamsDictionary.");
                    Debug.Break();
                }
            }
        }
        else
        {
            Debug.LogError("CamsManager: camsDataArr is null.");
            Debug.Break();
        }

        currentCam = CamEnum.Stage;
    }

    public void FlipCam()
    {
        if (isCamsDisabled)
        {
            return;
        }

        inCams = !inCams;
        Debug.Log("CameraFlip "  + inCams);

        if (inCams)
        {
            SwitchCam(currentCam);
        }

        OnCamFlip?.Invoke(inCams);
    }

    public void SwitchCam(CamEnum cam)
    {
        Debug.Log("Switching to cam: " + cam);
        if (cam == CamEnum.Invalid || cam == CamEnum.None)
        {
            Debug.LogError("CamsManager: Invalid cam switch attempted.");
            return;
        }

        currentCam = cam;

        var anims = new List<AnimatronicEnum>();
        var events = new List<EventData>();
        int foxyStage = AIManager.Singleton.foxyStage;

        if (cam == CamEnum.PirateCove)
        {
            if (foxyStage < 0 || foxyStage > 4)
            {
                Debug.LogError("CamsManager: Invalid foxy stage.");
                Debug.Break();
            }

            AnimatronicEnum[] animEnum = foxyStage == 4 ? new AnimatronicEnum[] { } : new AnimatronicEnum[] { AnimatronicEnum.Foxy };
            OnCamSwitch?.Invoke(cam, animEnum, new EventData[] { }, foxyStage);
            return;
        }

        EventData[] baseEvents = EnumCamsDictionary[cam].events;

        foreach (AnimatronicEnum anim in AIManager.Singleton.AnimLocations.Keys)
        {
            if (AIManager.Singleton.AnimLocations[anim] == cam)
            {
                anims.Add(anim);
            }
        }

        foreach (EventData e in baseEvents)
        {
            if (e.animsInvolved.Length > 0)
            {
                if (e.animsInvolved.Length != e.animsInvolved.Length)
                {
                    continue;
                }

                for (int i = 0; i < e.animsInvolved.Length; i++)
                {
                    AnimatronicEnum anim = e.animsInvolved[i];

                    if (anims.Contains(anim))
                    {
                        if (i == e.animsInvolved.Length - 1)
                        {
                            events.Add(e);
                            break;
                        }
                        continue;
                    }
                    else
                    {
                        Debug.Log("CamsManager: Event " + e.eventName + " has animsInvolved that are not in the camera.");
                        break;
                    }
                }
            }
        }

        OnCamSwitch?.Invoke(cam, anims.ToArray(), events.ToArray(), foxyStage);
    }

    private void OnOutage()
    {
        if (inCams)
        {
            FlipCam();
        }

        isCamsDisabled = true;
    }
}
