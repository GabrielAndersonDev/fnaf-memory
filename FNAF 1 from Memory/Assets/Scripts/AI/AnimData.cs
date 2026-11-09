using UnityEngine;

[CreateAssetMenu(fileName = "AnimData", menuName = "Scriptable Objects/AnimData")]
public abstract class AnimData : ScriptableObject
{
    public CamData startLocation;
    public CamData[] usableLocations;

    int difficulty;
    bool canMove;

    // may not be right type, will have to check when implementing
    public AnimationEvent jumpscare;

    public virtual void Move()
    {

    }

    public virtual void Jumpscare()
    {
       
    }
}
