using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapChanger.UI;

public abstract class Title(string name, string mod) : MagicUIWrapper(name, mod)
{
    public TextObject TitleText { get; private set; }

    public override void Initialize(LayoutRoot root)
    {
        TitleText = new(root, FullName)
        {
            TextAlignment = HorizontalAlignment.Left,
            ContentColor = Color.white,
            FontSize = 20,
            Font = MagicUI.Core.UI.TrajanBold,
            Padding = new(10f, 840f, 10f, 10f),
            Text = Mod,
        };
    }

    public override void Update()
    {
        if (Settings.MapModEnabled() && Settings.CurrentMode().Mod == Mod)
        {
            TitleText.Visibility = Visibility.Visible;
        }
        else
        {
            TitleText.Visibility = Visibility.Hidden;
        }

        TitleText.Text = Mod;
    }
}
