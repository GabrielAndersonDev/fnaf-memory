using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    // may remove if i get events working well idk
    public static GameUIManager Singleton { get; private set; }

    [SerializeField]
    GameObject officeObject;
    VisualElement officeUI;

    [SerializeField]
    GameObject camsObject;
    VisualElement camsUI;

    [SerializeField]
    GameObject baseGameUIObject;
    VisualElement baseGameUI;

    Box flipContainer;
    [SerializeField]
    VisualTreeAsset flipElementAsset;
    [SerializeField]
    Sprite flipSprite;

    [SerializeField]
    Sprite camNavAsset;

    TextElement powerText;
    Box powerUseSpriteContainer;
    TextElement hourText;

    TextElement currentCamText;

    Image camSprite;
    Box camSpriteContainer;
    Image camNavSprite;

    [SerializeField]
    private AnimatronicEnum[] animList;
    [SerializeField]
    private Animation[] jumpscareList;
    Dictionary<AnimatronicEnum, Animation> AnimJumpscareDictionary;

    [SerializeField]
    private Animation[] freddyOutageAnim;

    private void Awake()
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
    }

    void Start()
    {
        InitUI();
        InitAnimJumpDictionary();
    }

    public void InitUI()
    {
        camsUI = camsObject.GetComponent<UIDocument>().rootVisualElement;
        officeUI = officeObject.GetComponent<UIDocument>().rootVisualElement;
        baseGameUI = baseGameUIObject.GetComponent<UIDocument>().rootVisualElement;

        powerText = baseGameUI.Q<TextElement>("power-text");
        powerUseSpriteContainer = baseGameUI.Q<Box>("power-use-sprite-container");
        hourText = baseGameUI.Q<TextElement>("hour-text");

        flipContainer = baseGameUI.Q<Box>("flip-container");

        currentCamText = camsUI.Q<TextElement>("cam-name");
        camSpriteContainer = camsUI.Q<Box>("cam-sprite-container");
        camSprite = camsUI.Q<Image>("cam-sprite");
        camNavSprite = camsUI.Q<Image>("cam-nav-sprite");

        camNavSprite.sprite = camNavAsset;

        AddFlipElement();
        SetButtonControls();

        baseGameUI.BringToFront();

        CamsManager.Singleton.OnCamFlip += CamFlipUIToggle;
        CamFlipUIToggle(CamsManager.Singleton.inCams);

        CamsManager.Singleton.OnCamSwitch += UpdateCamUI;

        UpdatePower(100);
        PowerManager.Singleton.Power += UpdatePower;

        UpdateHour(12);
        NightManager.Singleton.HourChange += UpdateHour;

        GameManager.Singleton.KillPlayer += OnGameEnd;
        GameManager.Singleton.NightWin += OnGameEnd;
    }

    private void InitAnimJumpDictionary()
    {
        AnimJumpscareDictionary = new();

        for (var i = 0; i < animList.Count(); i++)
        {
            try
            {
                AnimJumpscareDictionary.Add(animList[i], jumpscareList[i]);
            }
            catch
            {
                Debug.LogError("GameUIManager: Error when initializing AnimJumpDictionary.");
                Debug.Break();
            }
        }
    }

    void AddFlipElement()
    {
        var flip = flipElementAsset.Instantiate();
        FlipElement element = flip.Q<FlipElement>();
        element.SetFlipSprite(flipSprite);

        element.RegisterCallback<PointerEnterEvent>(FlipPointerEnter);

        flipContainer.Add(element);
    }

    private void OnDisable()
    {
        flipContainer.UnregisterCallback<PointerEnterEvent>(FlipPointerEnter);
        CamsManager.Singleton.OnCamFlip -= CamFlipUIToggle;
        CamsManager.Singleton.OnCamSwitch -= UpdateCamUI;
        PowerManager.Singleton.Power -= UpdatePower;
        NightManager.Singleton.HourChange -= UpdateHour;
        GameManager.Singleton.KillPlayer -= OnGameEnd;
        GameManager.Singleton.NightWin -= OnGameEnd;
    }

    private void FlipPointerEnter(PointerEnterEvent evt)
    {
        CamsManager.Singleton.FlipCam();
    }

    void CamFlipUIToggle(bool toggle)
    {
        if (toggle)
        {
            camsUI.SetEnabled(true);
            officeUI.SetEnabled(false);
            camsUI.style.display = DisplayStyle.Flex;
            officeUI.style.display = DisplayStyle.None;
        }
        else
        {
            camsUI.SetEnabled(false);
            officeUI.SetEnabled(true);
            camsUI.style.display = DisplayStyle.None;
            officeUI.style.display = DisplayStyle.Flex;
        }
    }

    void SetButtonControls()
    {
        foreach (CamData data in CamsManager.Singleton.camDataArr)
        {
            string name = data.cam.ToString().ToLower().Replace(" ", "-") + "-btn";
                
            Button button = new()
            {
                name = data.cam.ToString().ToLower().Replace(" ", "-") + "-btn",
                text = data.CamName
            };

            button.clicked += () => CamsManager.Singleton.SwitchCam(data.cam);
            camNavSprite.Add(button);
        }

        if (camNavSprite.childCount < CamsManager.Singleton.camDataArr.Length)
        {
            Debug.LogError("GameUIManager: Not enough buttons for all cams.");
            Debug.Break();
        }
    }

    void UpdateCamUI(CamEnum cam, AnimatronicEnum[] anims, EventData[] events, int foxyStage)
    {
        if (cam == CamEnum.Invalid || cam == CamEnum.None || cam == CamEnum.Max)
        {
            Debug.LogError("GameUIManager: Invalid cam switch attempted.");
            return;
        }

        camSpriteContainer.Clear();

        CamData camData = CamsManager.Singleton.EnumCamsDictionary[cam];
        currentCamText.text = camData.CamName;
        Sprite baseCamSprite = camData.emptyCam;
        List<Sprite> animSprites = new();

        if (cam == CamEnum.PirateCove)
        {
            camSprite.sprite = camData.foxySprites[foxyStage];
            return;
        }

        if (cam == CamEnum.LHallway
            && anims.Contains(AnimatronicEnum.Foxy))
        {
            camSprite.sprite = baseCamSprite;

            Debug.LogError("Play Foxy running anim, unimplemented rn");
            return;
        }

        foreach (AnimSpriteData spriteData in camData.animSpriteDataArr)
        {
            if (anims.Contains(spriteData.anim))
            {
                animSprites.Add(spriteData.sprite);
            }
        }

        foreach (EventData e in events)
        {
            Sprite spriteRoll = e.EventRoll();

            if (spriteRoll != null)
            {
                switch (e.eventType)
                {
                    case (EventType.Background):
                        baseCamSprite = spriteRoll;
                        break;
                    case (EventType.Override):
                        baseCamSprite = spriteRoll;
                        return;
                    case (EventType.Single):
                        for (var i = 0; i < anims.Length; i++)
                        {
                            if (anims[i] == e.animsInvolved[0])
                            {
                                animSprites[i] = spriteRoll;
                                break;
                            }
                        }
                        break;
                    default:
                        Debug.LogError("GameUIManager: EventType in event " + e.eventName + " was invalid.");
                        Debug.Break();
                        break;
                }
            }
        }

        foreach (Sprite sprite in animSprites)
        {
            Image animSprite = new()
            {
                sprite = sprite,
                scaleMode = ScaleMode.ScaleToFit
            };

            camSpriteContainer.Add(animSprite);
        }

        camSprite.sprite = baseCamSprite;
    }

    private void UpdatePower(int newPower) 
    {
        powerText.text = newPower.ToString();
    }

    private void UpdateHour(int newHour)
    {
        hourText.text = newHour.ToString();
    }

    public void PlayJumpscare(AnimatronicEnum anim, bool isCamUp)
    {
        Debug.Log("You just got JUMPSCARED!!! LOSER!!!!");

        if (isCamUp)
        {
            // pull cam down forcefully
            CamsManager.Singleton.FlipCam();
        }

        // play jumpscare

        // end game
        GameManager.Singleton.OnKillPlayer();
    }

    private void OnGameEnd()
    {
        CamsManager.Singleton.OnCamFlip -= CamFlipUIToggle;
        CamsManager.Singleton.OnCamSwitch -= UpdateCamUI;
        PowerManager.Singleton.Power -= UpdatePower;
        NightManager.Singleton.HourChange -= UpdateHour;
        GameManager.Singleton.KillPlayer -= OnGameEnd;
        GameManager.Singleton.NightWin -= OnGameEnd;
    }
}
