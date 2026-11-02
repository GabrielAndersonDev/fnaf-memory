using UnityEngine;

[CreateAssetMenu(fileName = "LocationData", menuName = "Scriptable Objects/LocationData")]
public class LocationData : ScriptableObject
{
    Location[] connectedLocations;
    Sprite emptyLocation;

    AnimData[] potentialAnim;
    Sprite[] occupiedSpriteArr;

    Event[] events;
}
