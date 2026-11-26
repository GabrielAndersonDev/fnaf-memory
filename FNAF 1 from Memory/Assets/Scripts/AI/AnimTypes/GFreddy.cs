using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GFreddyData", menuName = "Scriptable Objects/GFreddyData")]
public class GFreddy : AnimData
{
    // this will be for while in office
    public override IEnumerator AnimatronicRoutine(WaitForSeconds waitTime, bool stallRun, WaitForSeconds stallTime)
    {
        AIManager.Singleton.UpdateAnimLocation(animatronic, CamEnum.Office);
        OfficeManager.Singleton.GFreddyToggle(true);

        yield return waitTime;

        GameManager.Singleton.GoldenFreddyKill();
    }
}
