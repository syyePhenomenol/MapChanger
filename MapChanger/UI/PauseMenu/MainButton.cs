using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapChanger.UI;

/// <summary>
/// A button that is persistently displayed in the main buttons grid on the pause menu.
/// </summary>
/// <param name="root"></param>
/// <param name="mod"></param>
/// <param name="name"></param>
public abstract class MainButton() : PauseMenuButton()
{
    protected override Button Build(PauseMenuLayout layout)
    {
        return new Button(layout, Name)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = MagicUI.Core.UI.TrajanBold,
            Margin = 8f,
        };
    }

    public override void Update()
    {
        base.Update();
        Button.BorderColor = GetBorderColor();
    }

    protected internal virtual Color GetBorderColor()
    {
        return Colors.GetColor(ColorSetting.UI_Neutral);
    }
}
