using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public abstract class BottomRowTextGrid : UIElementWrapper<MapLayout, DynamicUniformGrid>
{
    private List<BottomRowText> _texts;

    public override void Update()
    {
        foreach (var extraButton in _texts)
        {
            extraButton.Update();
        }
    }

    protected override DynamicUniformGrid Build(MapLayout layout)
    {
        DynamicUniformGrid grid =
            new(layout, Name)
            {
                ChildrenBeforeRollover = int.MaxValue,
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new(0f, 0f, 0f, 40f),
                HorizontalSpacing = 15f,
            };

        _texts = [];

        foreach (var text in GetTexts())
        {
            text.Initialize(layout);
            grid.Children.Add(text.Text);
            _texts.Add(text);
        }

        return grid;
    }

    protected internal abstract IEnumerable<BottomRowText> GetTexts();
}
