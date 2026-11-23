using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Singleton { get; private set; }
    [SerializeField]
    GameObject mainObj;
    VisualElement mainUI;

    [SerializeField]
    GameObject customObj;
    VisualElement customUI;

    Box startGameBox;
    Button startGameBtn;
    TextElement nightText;

    Button nightSixBtn;
    Button customNightBtn;
    Button quitGameBtn;

    Box starBox;
    [SerializeField]
    Sprite starSprite;

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

    private void Start()
    {
        InitUI();
    }

    private void InitUI()
    {
        mainUI = mainObj.GetComponent<UIDocument>().rootVisualElement;
        customUI = customObj.GetComponent<UIDocument>().rootVisualElement;

        startGameBox = mainUI.Q<Box>("start-game-box");
        startGameBtn = mainUI.Q<Button>("start-game-btn");
        nightText = mainUI.Q<TextElement>("night-text");

        nightSixBtn = mainUI.Q<Button>("night-six-btn");
        customNightBtn = mainUI.Q<Button>("custom-night-btn");
        quitGameBtn = mainUI.Q<Button>("quit-game-btn");

        starBox = mainUI.Q<Box>("star-box");

        CheckNightInfo();
        SetStarCount();
        SetButtonFunctions();
        ToggleCustomNight(false);
    }

    private void CheckNightInfo()
    {
        switch (SaveManager.Singleton.nightSaved)
        {
            case 0:
                if (SaveManager.Singleton.isNewSave)
                {
                    nightText.text = "";
                    startGameBtn.text = "New Game";
                }
                else
                {
                    nightText.text = "Night 1";
                    startGameBtn.text = "Continue";
                }

                nightSixBtn.style.display = DisplayStyle.None;
                customNightBtn.SetEnabled(false);
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                startGameBtn.text = "Continue";
                nightText.text = "Night " + SaveManager.Singleton.nightSaved;
                nightSixBtn.style.display = DisplayStyle.None;
                customNightBtn.SetEnabled(false);
                break;
            case 5:
                startGameBtn.text = "Continue";
                nightText.text = "Night 5";
                nightSixBtn.style.display = DisplayStyle.Flex;
                customNightBtn.SetEnabled(false);
                break;
            case 6:
            case 7:
                startGameBtn.text = "Continue";
                nightText.text = "Night 5";
                nightSixBtn.style.display = DisplayStyle.Flex;
                customNightBtn.SetEnabled(true);
                break;
            default:
                Debug.LogError("MainUIManager: Invalid night int: " + SaveManager.Singleton.nightSaved);
                Debug.Break();
                break;
        }
    }

    private void ToggleCustomNight(bool isCustom)
    {
        if (isCustom)
        {
            mainUI.SetEnabled(false);
            mainUI.style.display = DisplayStyle.None;

            customUI.SetEnabled(true);
            customUI.style.display = DisplayStyle.Flex;
        }
        else
        {
            mainUI.SetEnabled(true);
            mainUI.style.display = DisplayStyle.Flex;

            customUI.SetEnabled(false);
            customUI.style.display = DisplayStyle.None;
        }
    }

    private void SetStarCount()
    {
        int starCount = 0;

        if (SaveManager.Singleton.nightFiveCompleted) starCount++;
        if (SaveManager.Singleton.nightSixCompleted) starCount++;
        if (SaveManager.Singleton.maxModeCompleted) starCount++;

        for (var i = 0; i < starCount; i++)
        {
            Image newStar = new()
            {
                sprite = starSprite
            };

            starBox.Add(newStar);
        }
    }

    private void SetButtonFunctions()
    {
        startGameBtn.clicked += () => GameManager.Singleton.StartGame(StartGameClicked());
        nightSixBtn.clicked += () => GameManager.Singleton.StartGame(6);
        customNightBtn.clicked += () => ToggleCustomNight(true);
    }

    private int StartGameClicked()
    {
        switch (SaveManager.Singleton.nightSaved)
        {
            case 5: case 6: case 7:
                return 5;
            case 0:
                return 1;
            case 1: case 2: case 3: case 4:
                return SaveManager.Singleton.nightSaved;
            default:
                Debug.LogError("MainUIManager: Invalid night int.");
                Debug.Break();
                return -1;
        }
    }

    private void OnDisable()
    {
        startGameBtn.clicked -= () => GameManager.Singleton.StartGame(StartGameClicked());
        nightSixBtn.clicked -= () => GameManager.Singleton.StartGame(6);
        customNightBtn.clicked -= () => ToggleCustomNight(true);
    }
}