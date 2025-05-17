using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public enum WorldMapStackAlignment
{
    TopLeft,
    BottomLeft,
    TopRight,
}

public abstract class WorldMapStack : UIElementWrapper<WorldMapLayout, StackLayout>
{
    private List<WorldMapPanel> _panels;

    public abstract WorldMapStackAlignment Alignment { get; }
    public StackLayout Stack => Element;

    public override void Update()
    {
        foreach (var panel in _panels)
        {
            panel.Update();
        }
    }

    protected override StackLayout Build(WorldMapLayout root)
    {
        StackLayout stack =
            new(root, Name)
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = Alignment is WorldMapStackAlignment.TopLeft or WorldMapStackAlignment.BottomLeft
                    ? HorizontalAlignment.Left
                    : HorizontalAlignment.Right,
                VerticalAlignment = Alignment is WorldMapStackAlignment.TopLeft or WorldMapStackAlignment.TopRight
                    ? VerticalAlignment.Top
                    : VerticalAlignment.Bottom,
                Padding = Alignment switch
                {
                    WorldMapStackAlignment.TopLeft => new(160f, 170f, 0f, 0f),
                    WorldMapStackAlignment.BottomLeft => new(160f, 0f, 0f, 150f),
                    WorldMapStackAlignment.TopRight => new(0f, 170f, 160f, 0f),
                    _ => default,
                },
                Spacing = 10f,
            };

        _panels = [];
        foreach (var panel in GetPanels())
        {
            panel.Initialize(Layout, this);
            stack.Children.Add(panel.Panel);
            _panels.Add(panel);
        }

        return stack;
    }

    protected internal abstract IEnumerable<WorldMapPanel> GetPanels();
}
