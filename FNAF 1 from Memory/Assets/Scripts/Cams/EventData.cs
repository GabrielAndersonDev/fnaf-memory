using UnityEngine;

// may repurpose to have an enum for specific events
public enum EventType
{
    Invalid = -2,
    None = -1,
    First,
    Background = First,
    Override,
    Single,
    Max
}

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    public string eventName;
    public EventType eventType;
    public CamData camInvolved;
    public AnimatronicEnum[] animsInvolved;

    // is a result mandatory? calcs based on equal rates for number of event rates that exist
    public bool isMandatory;
    public int[] eventRates;
    public Sprite[] eventSprites;

    public Sprite EventRoll()
    {
        if (eventRates.Length < 1 || eventSprites.Length < 1)
        {
            Debug.LogError("EventData " + eventName + ": eventRates or eventSprites are less than one in length.");
            Debug.Break();
        }

        if (isMandatory)
        {
            int roll = Random.Range(0, eventRates[0]);
            return eventSprites[roll];
        }
        else
        {
            if (eventRates.Length != eventSprites.Length)
            {
                Debug.LogError("EventData " + eventName + ": eventRates and eventSprites are not the same length.");
                Debug.Break();
            }

            for (int i = 0; i < eventRates.Length; i++)
            {
                int roll = Random.Range(1, eventRates[i] + 1);

                if (roll == 1)
                {
                    return eventSprites[i];
                }
            }
        }

        return null;
    }
}