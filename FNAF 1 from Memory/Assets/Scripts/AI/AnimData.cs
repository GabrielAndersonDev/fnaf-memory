using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AnimData : ScriptableObject
{
    public string animName;
    public AnimatronicEnum animatronic;
    public CamEnum startLocation;
    public CamEnum returnLocation;
    public CamEnum[] usableLocations;
    public Sprite mugshot;

    public int difficulty;
    public bool isStalled;

    // may not be right type, will have to check when implementing
    public AnimationEvent jumpscare;

    public virtual void InitAnim(int dif)
    {
        difficulty = dif;
    }

    public virtual IEnumerator AnimatronicRoutine(WaitForSeconds waitTime, bool stallRun, WaitForSeconds stallTime)
    {
        isStalled = stallRun;

        while (!NightManager.Singleton.isNightOver)
        {
            WaitForSeconds time = isStalled ? stallTime : waitTime;
            
            yield return time;

            int rand = UnityEngine.Random.Range(1, 21);

            if (rand <= difficulty)
            {
                Move();
            }

            isStalled = false;
        }
    }

    public virtual void Move()
    {
        CamEnum currentCam = AIManager.Singleton.AnimLocations[animatronic];

        // figure out cam weights in proportion to current cam.

        switch (currentCam)
        {
            case CamEnum.Invalid: case CamEnum.None: case CamEnum.Max:
                Debug.LogError("AnimData " + animName + " returned unusable CamEnum: " + currentCam);
                Debug.Break();
                break;
            case CamEnum.LDoor:
                if (!OfficeManager.Singleton.leftDoor.isClosed)
                {
                    Debug.Log("Left door is open on movement opportunity");
                    if (CamsManager.Singleton.inCams)
                    {
                        AIManager.Singleton.UpdateAnimLocation(animatronic, CamEnum.Office);
                    }
                    else
                    {
                        OfficeManager.Singleton.isLDoorBtnDisabled = true;
                        CamsManager.Singleton.OnCamFlip += Jumpscare;
                    }
                }
                else
                {
                    AIManager.Singleton.UpdateAnimLocation(animatronic, returnLocation);
                }
                break;
            case CamEnum.RDoor:
                if (!OfficeManager.Singleton.rightDoor.isClosed)
                {
                    if (CamsManager.Singleton.inCams)
                    {
                        AIManager.Singleton.UpdateAnimLocation(animatronic, CamEnum.Office);
                    }
                    else
                    {
                        OfficeManager.Singleton.isRDoorBtnDisabled = true;
                        CamsManager.Singleton.OnCamFlip += Jumpscare;
                    }
                }
                else
                {
                    AIManager.Singleton.UpdateAnimLocation(animatronic, returnLocation);
                }
                break;
            case CamEnum.Office:
                Jumpscare(CamsManager.Singleton.inCams);
                break;
            default:
                AIManager.Singleton.UpdateAnimLocation(animatronic, SelectMoveCam(currentCam));
                break;
        }
    }

    public virtual CamEnum SelectMoveCam(CamEnum currentCam)
    {
        CamData camData = CamsManager.Singleton.EnumCamsDictionary[currentCam];

        List<CamEnum> camList = new();
        float difExpo = AIManager.Singleton.difficultyCurve.Evaluate(difficulty);

        float weightTotal = 0f;
        List<float> camWeightList = new();

        float priorBase = 0;

        // Adds cams based on validity, and totals their weights in same order. First cams should be smaller float.
        for (int i = 0; i < camData.connectedCams.Length; i++)
        {
            if (usableLocations.Contains(camData.connectedCams[i]))
            {
                camList.Add(camData.connectedCams[i]);

                float baseWeight = priorBase == 0 ? 1 : (priorBase + priorBase);
                priorBase = baseWeight;
                float camWeight = (float)Math.Pow(baseWeight, difExpo);

                weightTotal += camWeight;

                camWeightList.Add(weightTotal);
            }
        }

        float randResult = UnityEngine.Random.Range(0f, weightTotal);
        CamEnum selectedCam = CamEnum.None;

        for (int i = 0; i < camList.Count; i++)
        {
            if (randResult < camWeightList[i])
            {
                selectedCam = camList[i];
                break;
            }
        }

        if (selectedCam == CamEnum.None)
        {
            Debug.LogError("AnimData: Failed to choose new CamEnum locaation.");
            Debug.Break();
            return selectedCam;
        }

        return selectedCam;
    }

    public virtual void Jumpscare(bool isCamUp)
    {
        GameUIManager.Singleton.PlayJumpscare(animatronic, isCamUp);
    }

    public virtual IEnumerator FreddyOutage()
    {
        yield break;
    }
}
