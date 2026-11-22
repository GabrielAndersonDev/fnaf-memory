using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public abstract class AnimData : ScriptableObject
{
    public readonly static WaitForSeconds waitForSeconds5 = new(5f);

    public string animName;
    public int animID;
    public AnimatronicEnum animatronic;
    public CamEnum startLocation;
    public CamEnum returnLocation;
    public CamEnum[] usableLocations;

    public int difficulty;

    // may not be right type, will have to check when implementing
    public AnimationEvent jumpscare;

    public virtual void InitAnim(int dif)
    {
        difficulty = dif;
    }

    public virtual IEnumerator AnimatronicRoutine()
    {
        while (!NightManager.Singleton.isNightOver)
        {
            yield return waitForSeconds5;

            Debug.Log("Animatronic " + animName + " coroutine is running!");

            int rand = Random.Range(1, 20);

            if (rand <= difficulty)
            {
                Move();
            }
        }

        yield return null;
    }

    public virtual void Move()
    {
        CamEnum currentCam = AIManager.Singleton.AnimLocations[animatronic];

        switch (currentCam)
        {
            case CamEnum.Invalid: case CamEnum.None: case CamEnum.Max:
                Debug.LogError("AnimData " + animName + " returned unusable CamEnum: " + currentCam);
                Debug.Break();
                break;
            case CamEnum.LDoor:
                Debug.Log("Running left door:");
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
                CamData camData = CamsManager.Singleton.EnumCamsDictionary[currentCam];

                List<CamEnum> camList = new();

                foreach (CamEnum cam in camData.connectedCams)
                {
                    if (usableLocations.Contains(cam))
                    {
                        camList.Add(cam);
                    }
                }

                int selectedInt = Random.Range(0, camList.Count - 1);

                AIManager.Singleton.UpdateAnimLocation(animatronic, camList[selectedInt]);
                break;
        }
    }

    public virtual void Jumpscare(bool isCamUp)
    {
        GameUIManager.Singleton.PlayJumpscare(animatronic, isCamUp);
    }

    public virtual IEnumerator FreddyOutage()
    {
        yield return null;
    }
}
