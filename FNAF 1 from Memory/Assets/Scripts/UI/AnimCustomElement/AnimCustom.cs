using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AnimCustom : VisualElement
{
    private TextElement NameText => this.Q<TextElement>("name-text");
    private Image AnimSprite => this.Q<Image>("anim-img");
    //private Button DownBtn => this.Q<Button>("down-btn");
    //private TextElement DifText => this.Q<TextElement>("dif-text");
    //private Button UpBtn => this.Q<Button>("up-btn");

    public void InitAnimCustom(string name, Sprite sprite)
    {
        NameText.text = name;
        AnimSprite.sprite = sprite;
    }

    public AnimCustom() { }
}
