using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    /// <summary>
    /// A button that is persistently displayed in the main buttons grid on the pause menu.
    /// </summary>
    public abstract class MainButton(string name, string mod, int row, int column) : ButtonWrapper($"{mod} {name}")
    {
        public readonly string Mod = mod;
        public readonly int Row = row;
        public readonly int Column = column;

        public override void Make()
        {
            Button = new Button(PauseMenu.Root, Name)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BorderColor = Colors.GetColor(ColorSetting.UI_Borders),
                MinHeight = 28f,
                MinWidth = 95f,
                Font = MagicUI.Core.UI.TrajanBold,
                FontSize = 11,
                Margin = 0f
            }.WithProp(GridLayout.Row, Row).WithProp(GridLayout.Column, Column);

            Button.Click += OnClickInternal;
            Button.OnHover += OnHover;
            Button.OnUnhover += OnUnhover;
            PauseMenu.MainButtonsGrid.Children.Add(Button);
            PauseMenu.MainButtons.Add(this);
        }

        public override void Update()
        {
            if (Settings.MapModEnabled() && Settings.CurrentMode().Mod == Mod)
            {
                Button.Visibility = Visibility.Visible;
            }
            else
            {
                Button.Visibility = Visibility.Hidden;
            }
        }
    }
}
