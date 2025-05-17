using System;
using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// A toggleable panel of buttons in the pause menu.
/// </summary>
/// <param name="root"></param>
/// <param name="mod"></param>
/// <param name="name"></param>
/// <param name="rowSize"></param>
public abstract class ExtraButtonGrid() : PauseMenuElement<DynamicUniformGrid>()
{
    private List<ExtraButton> _buttons;

    public abstract int RowSize { get; }
    public DynamicUniformGrid Grid => Element;

    public override void Update()
    {
        foreach (var extraButton in _buttons)
        {
            extraButton?.Update();
        }
    }

    public override void Destroy()
    {
        foreach (var extraButton in _buttons)
        {
            extraButton?.Destroy();
        }

        Grid?.Destroy();
    }

    protected override DynamicUniformGrid Build(PauseMenuLayout layout)
    {
        DynamicUniformGrid grid =
            new(layout, Name)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Vertical,
                ChildrenBeforeRollover = RowSize,
                HorizontalSpacing = 2f,
                VerticalSpacing = 2f,
            };

        _buttons = [];

        foreach (var button in GetButtons())
        {
            button.Initialize(layout);
            grid.Children.Add(button.Button);
            _buttons.Add(button);
        }

        return grid;
    }

    protected internal abstract IEnumerable<ExtraButton> GetButtons();

    internal void Show()
    {
        Grid.Visibility = Visibility.Visible;
    }

    internal void Hide()
    {
        Grid.Visibility = Visibility.Collapsed;
    }

    internal void Toggle()
    {
        if (Grid.Visibility == Visibility.Collapsed)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}
