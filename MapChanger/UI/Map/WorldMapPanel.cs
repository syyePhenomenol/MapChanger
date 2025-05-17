using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using UnityEngine;

namespace MapChanger.UI;

public abstract class WorldMapPanel : UIElementWrapper<WorldMapLayout, Panel>
{
    public WorldMapStack Stack { get; private set; }
    public Panel Panel => Element;

    public override void Update()
    {
        if (Layout.GetElement(Name + " Background") is Image backgroundImage)
        {
            backgroundImage.Tint = GetBackgroundTint();
        }
    }

    internal void Initialize(WorldMapLayout worldMapLayout, WorldMapStack stack)
    {
        Stack = stack;
        base.Initialize(worldMapLayout);
    }

    protected override Panel Build(WorldMapLayout layout)
    {
        Panel panel =
            new(
                layout,
                Stack.Alignment is WorldMapStackAlignment.TopLeft or WorldMapStackAlignment.BottomLeft
                    ? SpriteManager.Instance.GetTexture("GUI.PanelLeft").ToSlicedSprite(200f, 50f, 100f, 50f)
                    : SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 200f, 50f),
                Name
            )
            {
                Borders = Stack.Alignment is WorldMapStackAlignment.TopLeft or WorldMapStackAlignment.BottomLeft
                    ? new(20f, 30f, 40f, 30f)
                    : new(40f, 30f, 20f, 30f),
                MinHeight = 0f,
                MinWidth = 0f,
                HorizontalAlignment = Stack.Alignment
                    is WorldMapStackAlignment.TopLeft
                        or WorldMapStackAlignment.BottomLeft
                    ? HorizontalAlignment.Left
                    : HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };

        return panel;
    }

    protected internal abstract Color GetBackgroundTint();
}
