using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FoxyData", menuName = "Scriptable Objects/FoxyData")]
public class Foxy : AnimData
{
    private static readonly WaitForSeconds waitForSeconds3 = new(3f);
    private Coroutine FoxyRunCoroutine;
    public override IEnumerator AnimatronicRoutine()
    {
        while (!NightManager.Singleton.isNightOver)
        {
            while (!CamsManager.Singleton.inCams
                && AIManager.Singleton.foxyStage != 4)
            {
                yield return waitForSeconds5;

                int rand = Random.Range(1, 20);

                if (rand <= difficulty)
                {
                    Move();
                }
            }

            yield return waitForSeconds3;
        }

        yield return null;
    }

    public override void Move()
    {
        if (AIManager.Singleton.foxyStage > 2)
        {
            AIManager.Singleton.foxyStage = 4;

            AIManager.Singleton.UpdateAnimLocation(AnimatronicEnum.Foxy, CamEnum.LHallway);
            CamsManager.Singleton.OnCamSwitch += CheckCamState;
            AIManager.Singleton.RunFoxyCoroutine(FoxyRunRoutine(false));
        }
        else
        {
            AIManager.Singleton.foxyStage++;
        }
    }

    private void CheckCamState(CamEnum cam, AnimatronicEnum[] anims, EventData[] events, int foxyStage)
    {
        if (cam == CamEnum.LHallway
            && anims.Contains(AnimatronicEnum.Foxy))
        {
            AIManager.Singleton.RunFoxyCoroutine(FoxyRunRoutine(true));
        }
    }

    private IEnumerator FoxyRunRoutine(bool isCamCheck)
    {
        if (isCamCheck)
        {
            yield return waitForSeconds3;
            FoxyDoorCheck();
        }
        else
        {
            yield return waitForSeconds5;
            FoxyDoorCheck();
        }

        CamsManager.Singleton.OnCamSwitch -= CheckCamState;
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

            AIManager.Singleton.UpdateAnimLocation(AnimatronicEnum.Foxy, CamEnum.PirateCove);
            AIManager.Singleton.foxyStage = 0;
        }
    }

    public override void Jumpscare(bool camUp)
    {
        base.Jumpscare(camUp);
        
    }
}
