using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Singleton { get; private set; }
    Player player;

    [SerializeField]
    GameObject officeObject;
    VisualElement officeUI;

    [SerializeField]
    GameObject camsObject;
    VisualElement camsUI;

    [SerializeField]
    VisualTreeAsset flipElement;

    TextElement officePower;
    TextElement camsPower;
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
        if (Player.Singleton != null)
        {
            player = Player.Singleton;
        }
        else
        {
            Debug.Log("Player Instance is not valid in GameUIManager.");
            Debug.Break();
        }

        officeUI = officeObject.GetComponent<UIDocument>().rootVisualElement;
        camsUI = camsObject.GetComponent<UIDocument>().rootVisualElement;

        officePower = officeUI.Q<TextElement>("power-text");
        camsPower = camsUI.Q<TextElement>("power-text");
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

        if (player.inCams)
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
        FlipElement flipElementCam = flipElement.Instantiate().Q<FlipElement>();
        FlipElement flipElementOffice = flipElement.Instantiate().Q<FlipElement>();

        camSprite.Add(flipElementCam);

    }

    void SetButtonControls()
    {
        
    }
}
