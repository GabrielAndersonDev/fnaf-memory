using UnityEngine;

[CreateAssetMenu(fileName = "LocationData", menuName = "Scriptable Objects/LocationData")]
public class CamData : ScriptableObject
{
    public int camID;
    public CamEnum cam;
    Sprite emptyCam;

    AnimData[] potentialAnim;
    Sprite[] occupiedSpriteArr;

    Event[] events;
}
