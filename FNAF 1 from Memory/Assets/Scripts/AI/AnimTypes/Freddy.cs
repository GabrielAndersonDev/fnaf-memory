using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FreddyData", menuName = "Scriptable Objects/FreddyData")]
public class Freddy : AnimData
{
    [SerializeField]
    CamEnum killCam;

    public override IEnumerator AnimatronicRoutine(int difficulty)
    {
        return base.AnimatronicRoutine(difficulty);
    }
}
