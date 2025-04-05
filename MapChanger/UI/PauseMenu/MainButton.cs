using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// A button that is persistently displayed in the main buttons grid on the pause menu.
/// </summary>
/// <param name="name"></param>
/// <param name="mod"></param>
/// <param name="row"></param>
/// <param name="column"></param>
public abstract class MainButton(string name, string mod, int row, int column) : ButtonWrapper(name, mod)
{
    public int Row { get; } = row;
    public int Column { get; } = column;

    protected override Button MakeButton(LayoutRoot root)
    {
        return new Button(root, Name)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            BorderColor = Colors.GetColor(ColorSetting.UI_Borders),
            MinHeight = 28f,
            MinWidth = 95f,
            Font = MagicUI.Core.UI.TrajanBold,
            FontSize = 11,
            Margin = 0f,
        }
            .WithProp(GridLayout.Row, Row)
            .WithProp(GridLayout.Column, Column);
    }

    public override void Update()
    {
        if (Settings.MapModEnabled() && Settings.CurrentMode().Mod == Mod)
        {
            Button.Visibility = Visibility.Visible;
        }
        else
        {
            Button.Visibility = Visibility.Hidden;
        }
    }
}
