using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public class PauseMenuTitle : PauseMenuElement<TextObject>
{
    private PauseMenuButton _overrideButton;

    public TextObject Text => Element;

    public override void Update()
    {
        var textFormat = _overrideButton?.GetHoverTextFormat() ?? GetDefaultTextFormat();
        Text.Text = textFormat.Text;
        Text.ContentColor = textFormat.Color;
        Text.FontSize = GetFontSize();
    }

    protected override TextObject Build(PauseMenuLayout layout)
    {
        return new(layout, Name) { TextAlignment = HorizontalAlignment.Left, Font = MagicUI.Core.UI.TrajanBold };
    }

    protected internal virtual TextFormat GetDefaultTextFormat()
    {
        return new(Layout.Mod.L(), Colors.GetColor(ColorSetting.UI_Neutral));
    }

    internal void SetOverrideButton(PauseMenuButton overrideButton)
    {
        _overrideButton = overrideButton;
    }

    private int GetFontSize()
    {
        return (int)(20f * ((float)MapChangerMod.GS.UIScale));
    }
}
