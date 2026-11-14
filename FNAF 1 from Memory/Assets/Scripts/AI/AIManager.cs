using System.Collections.Generic;
using UnityEngine;

public enum AnimatronicEnum
{
    Invalid = -2,
    None = -1,
    First,
    Bonnie = First,
    Freddy,
    Chica,
    Foxy,
    GoldenFreddy,
    Max
}

public class AIManager : MonoBehaviour
{
    public static AIManager Singleton { get; private set; }

    public AnimData[] animatronics;

    public Dictionary<AnimatronicEnum, CamEnum> AnimLocations;

    public int foxyStage;

    void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }

        PopulateAnimLocations();
    }

    void PopulateAnimLocations()
    {
        AnimLocations = new Dictionary<AnimatronicEnum, CamEnum>();

        foreach (var anim in animatronics)
        {
            AnimLocations.Add(anim.animatronic, anim.startLocation);
        }
    }

    public void UpdateAnimLocation(AnimatronicEnum anim, CamEnum newLocation)
    {
        if (AnimLocations.ContainsKey(anim))
        {
            AnimLocations[anim] = newLocation;
        }
        else
        {
            Debug.LogError($"AIManager: AnimLocations does not contain {anim}.");
            Debug.Break();
        }
    }
}
