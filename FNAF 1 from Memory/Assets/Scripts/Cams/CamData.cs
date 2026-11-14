using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationData", menuName = "Scriptable Objects/LocationData")]
public class CamData : ScriptableObject
{
    public int camID;
    public string CamName;
    public CamEnum cam;
    public Sprite emptyCam;

    [SerializeField]
    AnimatronicEnum[] potentialAnim;
    [SerializeField]
    Sprite[] occupiedSpriteArr;
    [SerializeField]
    int[] spritePriorityArr;

    public Sprite[] foxySprites;

    public Dictionary<AnimatronicEnum, Sprite> occupiedSpriteDictionary;
    public Dictionary<AnimatronicEnum, int> spritePriorityDictionary;

    [SerializeField]
    Event[] events;

    void Awake()
    {
        occupiedSpriteDictionary = new Dictionary<AnimatronicEnum, Sprite>();
        spritePriorityDictionary = new Dictionary<AnimatronicEnum, int>();

        for (int i = 0; i < potentialAnim.Length; i++)
        {
            occupiedSpriteDictionary.Add(potentialAnim[i], occupiedSpriteArr[i]);
            spritePriorityDictionary.Add(potentialAnim[i], spritePriorityArr[i]);
        }
    }
}
