using UnityEngine;

[CreateAssetMenu(fileName = "GFreddyEventData", menuName = "Scriptable Objects/EventData/GFreddy")]
public class GFreddyEvent : EventData
{
    public override Sprite EventRoll()
    {

        if (eventRates.Length < 1 || eventSprites.Length < 1)
        {
            Debug.LogError("EventData " + eventName + ": eventRates or eventSprites are less than one in length.");
            Debug.Break();
        }

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
                if (i == 1)
                {
                    AIManager.Singleton.GFreddyStart();
                    Debug.Log("Is GFreddy active: " + AIManager.Singleton.isGFreddyActive);
                }
                
                return eventSprites[i];
            }
        }

        return null;
    }
}
