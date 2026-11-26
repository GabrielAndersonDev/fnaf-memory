using UnityEngine;

[CreateAssetMenu(fileName = "AnimDifficulty", menuName = "Scriptable Objects/AnimDifficulty")]
public class AnimDifficulty : ScriptableObject
{
    public int night;
    public int[] difficulty;

    public void InitDifficulty(int night, int[] difficulties)
    {
        this.night = night;
        difficulty = difficulties;
    }
}