using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FoxyData", menuName = "Scriptable Objects/FoxyData")]
public class Foxy : AnimData
{
    public override void Move()
    {
        Debug.Log("Foxy is moving: " + AIManager.Singleton.foxyStage);

        if (AIManager.Singleton.foxyStage == 3)
        {
            Debug.Log("Foxy is at stage 4 move");
            AIManager.Singleton.foxyStage = 4;

            AIManager.Singleton.UpdateAnimLocation(AnimatronicEnum.Foxy, CamEnum.LHallway);
            AIManager.Singleton.SwapFoxyRoutine(true, false);
        }
        else
        {
            AIManager.Singleton.foxyStage++;
        }
    }

    public IEnumerator FoxyRunRoutine(WaitForSeconds waitTime)
    {
        Debug.Log("In foxy run routine");

        yield return waitTime;
        FoxyDoorCheck();

        AIManager.Singleton.foxyAlreadySeen = false;
    }

    private void FoxyDoorCheck()
    {
        if (!OfficeManager.Singleton.leftDoor.isClosed)
        {
            Jumpscare(CamsManager.Singleton.inCams);
        }
        else
        {
            PowerManager.Singleton.FoxyAttack();
            AIManager.Singleton.foxyStage = 0;
            AIManager.Singleton.SwapFoxyRoutine(false, false);
            AIManager.Singleton.UpdateAnimLocation(AnimatronicEnum.Foxy, CamEnum.PirateCove);
        }
    }
}
