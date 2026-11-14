using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
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

    InputAction point;

    TextElement powerText;
    Box powerUseSpriteContainer;
    TextElement hourText;

    TextElement currentCamText;

    Image camSprite;
    Box camSpriteContainer;
    Box camContainer;

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

        camContainer = camsUI.Q<Box>("cam-container");

        AddFlipElement();
        SetButtonControls();

        CamsManager.Singleton.OnCamFlip += CamFlipUIToggle;
        CamFlipUIToggle(CamsManager.Singleton.inCams);

        CamsManager.Singleton.OnCamSwitch += UpdateCamUI;
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
            Button button = new()
            {
                name = data.CamName.ToLower().Replace(" ", "-") + "-btn " + data.camID,
                text = data.CamName
            };

            button.clicked += () => CamsManager.Singleton.SwitchCam(data.cam);
            camContainer.Add(button);
        }

        if (camContainer.childCount < CamsManager.Singleton.camDataArr.Length)
        {
            Debug.LogError("GameUIManager: Not enough buttons for all cams.");
            Debug.Break();
        }
    }

    void UpdateCamUI(CamEnum cam, AnimatronicEnum[] anims, Event[] events, int? foxyStage)
    {
        if (cam == CamEnum.Invalid || cam == CamEnum.None || cam == CamEnum.Max)
        {
            Debug.LogError("GameUIManager: Invalid cam switch attempted.");
            return;
        }

        if (camSpriteContainer.childCount > 0)
        {
            camSpriteContainer.Clear();
        }

        CamData camData = CamsManager.Singleton.EnumCamsDictionary[cam];

        if (cam == CamEnum.PirateCove)
        {
            camSprite.sprite = camData.foxySprites[foxyStage];
            return;
        }

        if (anims.Length > 0)
        {
            List<Sprite> animSprites = new();
            List<int> animPriorities = new();

            foreach (AnimatronicEnum anim in anims)
            {
                if (camData.spritePriorityDictionary.ContainsKey(anim))
                {
                    int priority = camData.spritePriorityDictionary[anim];

                    if (animSprites.Count == 0)
                    {
                        animSprites.Add(camData.occupiedSpriteDictionary[anim]);
                        animPriorities.Add(priority);
                        continue;
                    }
                    
                    for (int i = 0; i < animSprites.Count; i++)
                    {
                        if (priority > animPriorities[i])
                        {
                            animSprites.Insert(i, camData.occupiedSpriteDictionary[anim]);
                            animPriorities.Insert(i, priority);
                            continue;
                        }
                    }

                    animSprites.Add(camData.occupiedSpriteDictionary[anim]);
                    animPriorities.Add(priority);
                }
                else
                {
                    Debug.LogError("GameUIManager: Anim " + anim + " not found in camData.spritePriorityDictionary.");
                    Debug.Break();
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
        }

        if (events.Length > 0)
        {
            // Handle events, not implemented yet
        }

        camSprite.sprite = camData.emptyCam;
    }
}
