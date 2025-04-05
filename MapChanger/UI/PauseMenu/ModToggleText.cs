using MagicUI.Core;

namespace MapChanger.UI;

internal class ModToggleText : Title
{
    public ModToggleText()
        : base("Press Ctrl-M to enable map mod", nameof(MapChangerMod)) { }

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
    }
}
