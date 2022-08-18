using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    /// <summary>
    /// Buttons that belong in a toggleable panel in the pause menu.
    /// </summary>
    public abstract class ExtraButton : ButtonWrapper
    {
        public readonly string Mod;

        public ExtraButton(string name, string mod) : base($"{mod} {name}")
        {
            Mod = mod;
        }

        public override void Make()
        {
            Button = new(PauseMenu.Root, Name)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Borderless = true,
                MinHeight = 28f,
                MinWidth = 85f,
                Content = Name,
                Font = MagicUI.Core.UI.TrajanNormal,
                FontSize = 11,
                Margin = 0f
            };

            Button.Click += OnClickInternal;
        }
    }
}
