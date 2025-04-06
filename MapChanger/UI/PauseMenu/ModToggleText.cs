using MagicUI.Core;
using MapChanger.Input;

namespace MapChanger.UI;

internal class ModToggleText : Title
{
    public ModToggleText()
        : base("Mod Toggle Text", nameof(MapChangerMod)) { }

    public override void Update()
    {
        if (!Settings.MapModEnabled())
        {
            TitleText.Visibility = Visibility.Visible;
        }
        else
        {
            TitleText.Visibility = Visibility.Hidden;
        }

        TitleText.Text = $"Press {ModEnabledInput.Instance.GetBindingsText()} to enable map mod";
    }
}
