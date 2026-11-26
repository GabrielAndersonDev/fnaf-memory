using System.Collections.Generic;
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

    List<Box> difBoxes;

    Button customBackBtn;
    Button customStartBtn;

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

        difBoxes = customUI.Query<Box>(className: "dif-box").ToList();

        customBackBtn = customUI.Q<Button>("back-btn");
        customStartBtn = customUI.Q<Button>("custom-start-btn");

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

        switch (SaveManager.Singleton.nightSaved)
        {
            case 5:
                starCount = 1;
                break;
            case 6:
                starCount = 2;
                break;
            case 7:
                starCount = 3;
                break;
            default:
                break;
        }

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
        AnimDifBtnAssign(true);
        customBackBtn.clicked += () => ToggleCustomNight(false);
        customStartBtn.clicked += () => OnCustomStartBtn();
        quitGameBtn.clicked += () => Application.Quit();
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

    private void PopulateAnimCustom()
    {
        foreach ()
    }

    private void AnimDifBtnAssign(bool setOn)
    {
        foreach (Box difBox in difBoxes)
        {
            Button upBtn = difBox.Q<Button>(className: "up-btn");
            Button downBtn = difBox.Q<Button>(className: "down-btn");
            TextElement difElement = difBox.Q<TextElement>(className: "dif");

            if (setOn)
            {
                upBtn.clicked += () => ChangeDifCount(difElement, true);
                downBtn.clicked += () => ChangeDifCount(difElement, false);
            }
            else
            {
                upBtn.clicked -= () => ChangeDifCount(difElement, true);
                downBtn.clicked -= () => ChangeDifCount(difElement, false);
            }
            
        }
    }

    private void ChangeDifCount(TextElement difElement, bool isAdd)
    {
        if (int.TryParse(difElement.text, out int dif))
        {
            if (isAdd
                && dif < 20)
            {
                difElement.text = (dif + 1).ToString();
            }
            else if (!isAdd
                && dif > 0)
            {
                difElement.text = (dif - 1).ToString();
            }

            if (dif > 20
                || dif < 0)
            {
                difElement.text = dif > 20 ? "20" : "0";
            }
        }
        else
        {
            Debug.LogError("MainUIManager: custom difficulty int is invalid.");
            Debug.Break();
        }
    }

    private void OnCustomStartBtn()
    {
        int bonnie = 0;
        int chica =  0;
        int foxy = 0;
        int freddy = 0;

        foreach (Box difBox in difBoxes)
        {
            TextElement difElement = difBox.Q<TextElement>(className: "dif");

            if (int.TryParse(difElement.text, out int dif))
            {
                if (difElement.ClassListContains("bonnie"))
                {
                    bonnie = dif;
                    continue;
                }

                if (difElement.ClassListContains("chica"))
                {
                    chica = dif;
                    continue;
                }

                if (difElement.ClassListContains("freddy"))
                {
                    freddy = dif;
                    continue;
                }

                if (difElement.ClassListContains("foxy"))
                {
                    foxy = dif;
                    continue;
                }
            }
        }

        GameManager.Singleton.StartCustomNight(bonnie, chica, freddy, foxy);
    }

    private void OnDisable()
    {
        startGameBtn.clicked -= () => GameManager.Singleton.StartGame(StartGameClicked());
        nightSixBtn.clicked -= () => GameManager.Singleton.StartGame(6);
        customNightBtn.clicked -= () => ToggleCustomNight(true);
        AnimDifBtnAssign(false);
        customBackBtn.clicked -= () => ToggleCustomNight(false);
        customStartBtn.clicked -= () => OnCustomStartBtn();
        quitGameBtn.clicked -= () => Application.Quit();
    }
}