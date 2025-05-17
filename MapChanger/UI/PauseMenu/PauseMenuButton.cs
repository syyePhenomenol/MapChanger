using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// Base class for all pause menu buttons.
/// </summary>
public abstract class PauseMenuButton : PauseMenuElement<Button>
{
    public Button Button => Element;

    public override void Initialize(PauseMenuLayout pauseMenuLayout)
    {
        base.Initialize(pauseMenuLayout);

        Button.Click += OnClickInternal;
        Button.OnHover += OnHover;
        Button.OnUnhover += OnUnhover;
    }

    public override void Update()
    {
        var textFormat = GetTextFormat();
        Button.Content = textFormat.Text;
        Button.ContentColor = textFormat.Color;

        var scaleFactor = (float)MapChangerMod.GS.UIScale;
        Button.FontSize = (int)(12f * scaleFactor);
        Button.MinHeight = 28f * scaleFactor;
        Button.MinWidth = 95f * scaleFactor;
    }

    protected internal virtual void OnClick() { }

    protected internal virtual TextFormat? GetHoverTextFormat()
    {
        return null;
    }

    protected internal abstract TextFormat GetTextFormat();

    private void OnClickInternal(Button button)
    {
        OnClick();
        Layout.Update();
    }

    private void OnHover(Button button)
    {
        Layout.SetTitleOverrideButton(this);
        Layout.Update();
    }

    private void OnUnhover(Button button)
    {
        Layout.ResetTitle();
        Layout.Update();
    }
}
