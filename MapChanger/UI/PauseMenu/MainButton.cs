using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    /// <summary>
    /// A button that is persistently displayed in the main buttons grid on the pause menu.
    /// </summary>
    public abstract class MainButton : ButtonWrapper
    {
        public readonly string Mod;
        public readonly int Row;
        public readonly int Column;

        public MainButton(string name, string mod, int row, int column) : base($"{mod} {name}")
        {
            Mod = mod;
            Row = row;
            Column = column;
        }

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
