using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FreddyMenu : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite[] freddySprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator MainBackgroundAnim()
    {
        spriteRenderer.sprite = freddySprites[0];
        while (true)
        {
            WaitForSeconds waitTime = new(Random.Range(2f, 4f));
            yield return waitTime;

            int twitchCount = (int)Random.Range(1, 5);

            for (int i = 0; i < twitchCount; i++)
            {
                int sprite = Random.Range(0, freddySprites.Length);
                float count = Random.Range(0.1f, 0.4f);
                WaitForSeconds time = new(count);
                yield return time;
                spriteRenderer.sprite = freddySprites[sprite];
            }

            if (spriteRenderer.sprite != freddySprites[0])
            {
                float finalCount = Random.Range(0.1f, 0.3f);
                WaitForSeconds finalTime = new(finalCount);
                yield return finalTime;
                spriteRenderer.sprite = freddySprites[0];
            }
        }
    }

    public void DisableFreddyBG()
    {
        spriteRenderer.sprite = null;
    }
}
