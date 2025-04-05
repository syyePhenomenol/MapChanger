using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// A toggleable panel of buttons in the pause menu.
/// </summary>
/// <param name="name"></param>
/// <param name="mod"></param>
/// <param name="buttons"></param>
/// <param name="leftPadding"></param>
/// <param name="rowSize"></param>
public class ExtraButtonPanel(string name, string mod, IEnumerable<ExtraButton> buttons, float leftPadding, int rowSize)
    : MagicUIWrapper(name, mod)
{
    private readonly List<ExtraButton> _buttons = [.. buttons];

    public DynamicUniformGrid Grid { get; private set; }

    public ReadOnlyCollection<ExtraButton> Buttons => new(_buttons);

    public override void Initialize(LayoutRoot root)
    {
        Grid = new(root, FullName)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Orientation = Orientation.Vertical,
            Padding = new(leftPadding, 865f, 10f, 10f),
            HorizontalSpacing = 0f,
            VerticalSpacing = 0f,
            ChildrenBeforeRollover = rowSize,
        };

        foreach (var eb in _buttons)
        {
            eb.Initialize(root);
            Grid.Children.Add(eb.Button);
        }
    }

    public void Show()
    {
        PauseMenu.HideOtherPanels(this);
        Grid.Visibility = Visibility.Visible;
    }

    public void Hide()
    {
        Grid.Visibility = Visibility.Hidden;
    }

    public void Toggle()
    {
        if (Grid.Visibility == Visibility.Hidden)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public override void Update()
    {
        if (!Settings.MapModEnabled() || Settings.CurrentMode().Mod != Mod)
        {
            Hide();
        }

        foreach (var extraButton in _buttons)
        {
            extraButton.Update();
        }
    }
}
