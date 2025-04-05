using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapChanger.UI;

public class Title(string name, string mod) : MagicUIWrapper(name, mod)
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
            Text = Name,
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

        UpdateText();
    }

    public virtual void UpdateText()
    {
        TitleText.Text = Name;
    }
}
