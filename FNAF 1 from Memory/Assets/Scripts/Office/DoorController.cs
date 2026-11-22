using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(SpriteRenderer))]
public class DoorController : MonoBehaviour
{
    public bool isClosed;
    public bool isLightOn;
    bool animPresent;

    [SerializeField]
    bool isLeft;

    SpriteRenderer doorSpriteRenderer;

    [SerializeField]
    Sprite openDoor;
    [SerializeField]
    Sprite closedDoor;

    [SerializeField]
    SpriteRenderer lightSpriteRenderer;
    [SerializeField]
    SpriteRenderer animSpriteRenderer;

    [SerializeField]
    Sprite lightSprite;
    [SerializeField]
    Sprite animSprite;
    [SerializeField]
    Sprite altWindowSpriteRenderer;

    private void Start()
    {
        doorSpriteRenderer = GetComponent<SpriteRenderer>();
        AIManager.Singleton.AnimAtDoor += AnimatronicCheck;
        OfficeManager.Singleton.BtnPressed += ToggleDoor;
        PowerManager.Singleton.PowerOutage += OnOutage;

        doorSpriteRenderer.sprite = openDoor;
    }

    private void OnDisable()
    {
        AIManager.Singleton.AnimAtDoor -= AnimatronicCheck;
        OfficeManager.Singleton.BtnPressed -= ToggleDoor;
        PowerManager.Singleton.PowerOutage -= OnOutage;
    }

    private void AnimatronicCheck(bool isLeft, bool isThere)
    {
        if (isLeft == this.isLeft)
        {
            if (isThere)
            {
                animPresent = true;
            }
            else
            {
                animPresent = false;
            }
        }

        SetSprites(false, false);
    }

    private void ToggleDoor(bool isLeft, bool isLight, bool isPressed)
    {
        if (isLeft != this.isLeft)
        {
            return;
        }

        if (isLight)
        {
            isLightOn = isPressed;
        }
        else
        {
            isClosed = isPressed;
        }

        SetSprites(false, false);
    }

    public void SetSprites(bool isFlicker, bool isOn)
    {
        bool lightOn = isLightOn;

        if (isFlicker)
        {
            lightOn = isOn;
        }

        doorSpriteRenderer.sprite = isClosed ? closedDoor : openDoor;

        lightSpriteRenderer.sprite = lightOn ? lightSprite : null;

        if (animPresent && lightOn)
        {
            if (isLeft)
            {
                animSpriteRenderer.sprite = animSprite;
            }
            else
            {
                animSpriteRenderer.sprite = isClosed ? altWindowSpriteRenderer : animSprite;
            }
        }
        else
        {
            animSpriteRenderer.sprite = null;
        }
    }

    private void OnOutage()
    {
        isLightOn = false;
        isClosed = false;
        animPresent = false;
        SetSprites(false, false);

        AIManager.Singleton.AnimAtDoor -= AnimatronicCheck;
        OfficeManager.Singleton.BtnPressed -= ToggleDoor;
        PowerManager.Singleton.PowerOutage -= OnOutage;
    }
}
