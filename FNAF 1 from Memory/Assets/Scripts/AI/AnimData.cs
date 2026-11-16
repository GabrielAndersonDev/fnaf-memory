using UnityEngine;

public abstract class AnimData : ScriptableObject
{
    public string animName;
    public int animID;
    public AnimatronicEnum animatronic;
    public CamEnum startLocation;
    public CamEnum[] usableLocations;

    // may not be right type, will have to check when implementing
    public AnimationEvent jumpscare;

    public virtual void Move()
    {

    }

    public virtual void Jumpscare()
    {
       
    }
}
