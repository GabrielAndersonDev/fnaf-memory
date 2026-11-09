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
    Max
}

public class CamsManager : MonoBehaviour
{
    public static CamsManager Singleton {  get; private set; }
    public Dictionary<CamEnum, CamData> EnumCamsDictionary;

    [SerializeField]
    CamData[] camDataArr;

    public CamEnum currentCam;
    public bool inCams;

    public event Action<bool> OnCamFlip;

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

    public void AddFlipEvent(FlipElement flipElement)
    {
        flipElement.TriggerCamFlip += FlipCam;
    }

    void FlipCam()
    {
        inCams = !inCams;
        Debug.Log("CameraFlip");

        OnCamFlip?.Invoke(inCams);
    }

    public void SwitchCam(CamEnum cam)
    {

    }
}
