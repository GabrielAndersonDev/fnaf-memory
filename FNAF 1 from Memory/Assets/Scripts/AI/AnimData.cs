using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public abstract class AnimData : ScriptableObject
{
    public string animName;
    public int animID;
    public AnimatronicEnum animatronic;
    public CamEnum startLocation;
    public CamEnum returnLocation;
    public CamEnum[] usableLocations;

    // may not be right type, will have to check when implementing
    public AnimationEvent jumpscare;

    public virtual IEnumerator AnimatronicRoutine(int difficulty)
    {
        while (NightManager.Singleton.isNightOver)
        {
            yield return new WaitForSeconds(5f);

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
                if (OfficeManager.Singleton.isLDoorOpen)
                {
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
                if (OfficeManager.Singleton.isRDoorOpen)
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

    public virtual void Jumpscare(bool camUp)
    {
        if (camUp)
        {
            // Play cam pull down + jumpscare
        }
        else
        {
            // Just jumpscare
        }
        Debug.LogError("Jumpscare is unimplemented pensive");
    }
}
