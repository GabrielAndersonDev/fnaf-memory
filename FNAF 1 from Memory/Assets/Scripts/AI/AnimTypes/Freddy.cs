using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FreddyData", menuName = "Scriptable Objects/FreddyData")]
public class Freddy : AnimData
{
    public override void Move()
    {
        CamEnum currentCam = AIManager.Singleton.AnimLocations[animatronic];

        // Freddy laugh

        switch (currentCam)
        {
            case CamEnum.Invalid:
            case CamEnum.None:
            case CamEnum.Max:
            case CamEnum.LDoor:
            case CamEnum.RDoor:
                Debug.LogError("AnimData " + animName + " returned unusable CamEnum: " + currentCam);
                Debug.Break();
                break;
            case CamEnum.FourB:
                if (!OfficeManager.Singleton.rightDoor.isClosed)
                {
                    if (CamsManager.Singleton.inCams
                        && CamsManager.Singleton.currentCam != CamEnum.FourB)
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

    public override IEnumerator FreddyOutage()
    {
        for (var freddyStage = 0; freddyStage < 4; freddyStage++)
        {
            float time = Random.Range(3f, 10f);
            Debug.Log(time);
            yield return OfficeManager.Singleton.FreddyOutageAnim(time, freddyStage);
        }

        Jumpscare(false);
    }
}
