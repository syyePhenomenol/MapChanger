using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public abstract class BottomRowText : MapUILayer
{
    protected virtual float MinSpacing { get; } = 200f;
    protected virtual string[] TextNames { get; } = [];
    protected Dictionary<string, TextObject> MapTexts { get; private set; }

    protected override bool Condition()
    {
        return Settings.MapModEnabled() && (States.WorldMapOpen || States.QuickMapOpen);
    }

    public override void BuildLayout()
    {
        MapTexts = [];

        GridLayout grid =
            new(Root, $"{GetType().Name} + Grid")
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new(20f),
            };

        for (var i = 0; i < TextNames.Length; i++)
        {
            var textObject = new TextObject(Root, TextNames[i])
            {
                Text = TextNames[i],
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Font = MagicUI.Core.UI.TrajanNormal,
                FontSize = 16,
            }.WithProp(GridLayout.Column, i);

            MapTexts[TextNames[i]] = textObject;
        }

        foreach (var text in MapTexts.Values)
        {
            grid.ColumnDefinitions.Add(new GridDimension(MinSpacing, GridUnit.AbsoluteMin));
            grid.Children.Add(text);
        }
    }
}
