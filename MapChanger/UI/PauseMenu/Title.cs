using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapChanger.UI
{
    public class Title
    {
        public readonly string Mod;
        public TextObject TitleText { get; private set; }

        public Title(string mod)
        {
            Mod = mod;
        }

        public void Make()
        {
            TitleText = new(PauseMenu.Root, $"{Mod} Title")
            {
                TextAlignment = HorizontalAlignment.Left,
                ContentColor = Color.white,
                FontSize = 20,
                Font = MagicUI.Core.UI.TrajanBold,
                Padding = new(10f, 840f, 10f, 10f),
                Text = Mod,
            };

            PauseMenu.Titles.Add(this);
        }

        public virtual void Update()
        {
            if (Settings.MapModEnabled() && Settings.CurrentMode().Mod == Mod)
            {
                TitleText.Visibility = Visibility.Visible;
            }
            else
            {
                TitleText.Visibility = Visibility.Hidden;
            }
        }
    }
}
