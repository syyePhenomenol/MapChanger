using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    /// <summary>
    /// A toggleable panel of buttons in the pause menu.
    /// </summary>
    public abstract class ExtraButtonPanel(string name, string mod, float leftPadding, int rowSize)
    {
        public readonly string Name = name;
        public readonly string Mod = mod;

        protected List<ExtraButton> ExtraButtons { get; private set; }

        public DynamicUniformGrid ExtraButtonsGrid { get; private set; }

        public void Make()
        {
            ExtraButtonsGrid = new(PauseMenu.Root, $"{Mod} {Name}")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Vertical,
                Padding = new(leftPadding, 865f, 10f, 10f),
                HorizontalSpacing = 0f,
                VerticalSpacing = 0f,
                ChildrenBeforeRollover = rowSize
            };

            ExtraButtons = [];
            MakeButtons();

            PauseMenu.ExtraButtonPanels.Add(this);
        }

        protected abstract void MakeButtons();

        public void Show()
        {
            PauseMenu.HideOtherPanels(this);
            ExtraButtonsGrid.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            ExtraButtonsGrid.Visibility = Visibility.Hidden;
        }

        public void Toggle()
        {
            if (ExtraButtonsGrid.Visibility == Visibility.Hidden)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Update()
        {
            if (!Settings.MapModEnabled() || Settings.CurrentMode().Mod != Mod)
            {
                Hide();
            }

            foreach (ExtraButton extraButton in ExtraButtons)
            {
                extraButton.Update();
            }
        }
    }
}
