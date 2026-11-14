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

    public event Action<bool> OnCamFlip;

    // event will probably change when i get those working, int is foxy stages
    public event Action<CamEnum, AnimatronicEnum[], Event[], int?> OnCamSwitch;

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

        PopulateCamDictionary();
    }

    void PopulateCamDictionary()
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
    }

    public void FlipCam()
    {
        inCams = !inCams;
        Debug.Log("CameraFlip "  + inCams);

        OnCamFlip?.Invoke(inCams);
    }

    public void SwitchCam(CamEnum cam)
    {
        if (cam == CamEnum.Invalid || cam == CamEnum.None)
        {
            Debug.LogError("CamsManager: Invalid cam switch attempted.");
            return;
        }

        currentCam = cam;

        var anims = new List<AnimatronicEnum>();
        var foxyStage = (int?)null;

        if (cam == CamEnum.PirateCove)
        {
            AnimatronicEnum[] animEnum = foxyStage == 3 ? new AnimatronicEnum[] { } : new AnimatronicEnum[] { AnimatronicEnum.Foxy };
            OnCamSwitch?.Invoke(cam, animEnum, null, foxyStage);
            return;
        }

        foreach (AnimatronicEnum anim in AIManager.Singleton.AnimLocations.Keys)
        {
            if (AIManager.Singleton.AnimLocations[anim] == cam)
            {
                anims.Add(anim);
            }
        }

        OnCamSwitch?.Invoke(cam, anims.ToArray(), null, foxyStage);
    }
}
