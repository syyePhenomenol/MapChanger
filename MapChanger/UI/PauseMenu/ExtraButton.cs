using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// A button that belongs in a toggleable panel in the pause menu.
/// </summary>
/// <param name="root"></param>
/// <param name="mod"></param>
/// <param name="name"></param>
public abstract class ExtraButton : PauseMenuButton
{
    protected override Button Build(PauseMenuLayout layout)
    {
        return new Button(layout, Name)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Borderless = true,
            Font = MagicUI.Core.UI.TrajanNormal,
            Margin = 8f,
        };
    }
}
