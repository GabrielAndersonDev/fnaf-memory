using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    // may remove if i get events working well idk
    public static GameUIManager Singleton { get; private set; }

    VisualElement baseGameUI;

    [SerializeField]
    GameObject officeObject;
    VisualElement officeUI;

    [SerializeField]
    GameObject camsObject;
    VisualElement camsUI;

    [SerializeField]
    VisualTreeAsset flipElement;
    [SerializeField]
    Sprite flipSprite;

    TextElement powerText;
    Box powerUseSpriteContainer;
    TextElement hourText;
    Box flipContainer;

    TextElement currentCamText;

    Image camSprite;
    Button bathroomBtn;
    Button fourBBtn;
    Button janitorBtn;
    Button kitchenBtn;
    Button lHallwayBtn;
    Button rHallwayBtn;
    Button partsServiceBtn;
    Button partyBtn;
    Button pirateCoveBtn;
    Button stageBtn;

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

        InitUI();
    }

    public void InitUI()
    {
        baseGameUI = gameObject.GetComponent<UIDocument>().rootVisualElement;
        officeUI = officeObject.GetComponent<UIDocument>().rootVisualElement;
        camsUI = camsObject.GetComponent<UIDocument>().rootVisualElement;

        powerText = baseGameUI.Q<TextElement>("power-text");
        powerUseSpriteContainer = baseGameUI.Q<Box>("power-use-sprite-container");
        hourText = baseGameUI.Q<TextElement>("hour-text");
        flipContainer = baseGameUI.Q<Box>("flip-container");

        currentCamText = camsUI.Q<TextElement>("cam-name");
        camSprite = camsUI.Q<Image>("cam-sprite");

        bathroomBtn = camsUI.Q<Button>("bathroom-btn");
        fourBBtn = camsUI.Q<Button>("fourb-btn");
        janitorBtn = camsUI.Q<Button>("janitor-btn");
        kitchenBtn = camsUI.Q<Button>("kitchen-btn");
        lHallwayBtn = camsUI.Q<Button>("l-hallway-btn");
        rHallwayBtn = camsUI.Q<Button>("r-hallway-btn");
        partsServiceBtn = camsUI.Q<Button>("parts-service-btn");
        partyBtn = camsUI.Q<Button>("party-btn");
        pirateCoveBtn = camsUI.Q<Button>("pirate-cove-btn");
        stageBtn = camsUI.Q<Button>("stage-btn");


        AddFlipElement();
        CamsManager.Singleton.OnCamFlip += CamFlipUIToggle;

        CamFlipUIToggle(CamsManager.Singleton.inCams);
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

    void AddFlipElement()
    {
        FlipElement flipElementInstance = flipElement.Instantiate().Q<FlipElement>();
        flipElementInstance.Q<Image>("flip-sprite").sprite = flipSprite;
        CamsManager.Singleton.AddFlipEvent(flipElementInstance);

        flipContainer.Add(flipElementInstance);
    }

    void SetButtonControls()
    {
        
    }
}
