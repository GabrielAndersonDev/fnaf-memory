using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AnimSpriteData
{
    public AnimatronicEnum anim;
    public Sprite sprite;
    // may need to add specific animations in future?
}

[CreateAssetMenu(fileName = "LocationData", menuName = "Scriptable Objects/LocationData")]
public class CamData : ScriptableObject
{
    public int camID;
    public string CamName;
    public CamEnum cam;
    public Sprite emptyCam;

    public AnimSpriteData[] animSpriteDataArr;

    public Sprite[] foxySprites;

    public EventData[] events;
}
