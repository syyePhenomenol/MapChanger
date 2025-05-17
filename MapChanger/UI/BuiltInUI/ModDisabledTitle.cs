using MapChanger.Input;

namespace MapChanger.UI;

internal class ModDisabledTitle : PauseMenuTitle
{
    protected internal override TextFormat GetDefaultTextFormat()
    {
        return new(
            $"Press {ModEnabledInput.Instance.GetBindingsText()} to enable map mod",
            Colors.GetColor(ColorSetting.UI_Neutral)
        );
    }
}
