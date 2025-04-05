using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// Buttons that belong in a toggleable panel in the pause menu.
/// </summary>
/// <param name="name"></param>
/// <param name="mod"></param>
public abstract class ExtraButton(string name, string mod) : ButtonWrapper(name, mod)
{
    protected override Button MakeButton(LayoutRoot root)
    {
        return new Button(root, FullName)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Borderless = true,
            MinHeight = 28f,
            MinWidth = 85f,
            Content = Name,
            Font = MagicUI.Core.UI.TrajanNormal,
            FontSize = 11,
            Margin = 0f,
        };
    }
}
