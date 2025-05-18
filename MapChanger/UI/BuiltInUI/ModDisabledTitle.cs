using MapChanger.Input;

namespace MapChanger.UI;

internal class ModDisabledTitle : PauseMenuTitle
{
    protected internal override TextFormat GetDefaultTextFormat()
    {
        return new(
            $"{"Press".L()} {ModEnabledInput.Instance.GetBindingsText()} {"to enable map mod".L()}",
            Colors.GetColor(ColorSetting.UI_Neutral)
        );
    }
}
