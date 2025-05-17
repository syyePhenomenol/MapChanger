using System;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public abstract class BottomRowText : UIElementWrapper<MapLayout, TextObject>
{
    public TextObject Text => Element;

    public override void Update()
    {
        var textFormat = GetTextFormat();
        Text.Text = textFormat.Text;
        Text.ContentColor = textFormat.Color;
        Text.FontSize = (int)Math.Min(16f * MapChangerMod.GS.UIScale, 22f);
    }

    protected override TextObject Build(MapLayout layout)
    {
        return new TextObject(layout, Name)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = MagicUI.Core.UI.TrajanNormal,
        };
    }

    protected internal abstract TextFormat GetTextFormat();
}
