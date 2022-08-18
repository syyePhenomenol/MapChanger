using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public class PauseMenu : HookModule
    {
        internal static LayoutRoot Root { get; private set; }
        internal static GridLayout MainButtonsGrid { get; private set; }

        internal static List<Title> Titles { get; private set; } = new();

        internal static List<MainButton> MainButtons { get; private set; } = new();
        internal static List<ExtraButtonPanel> ExtraButtonPanels { get; private set; } = new();

        public override void OnEnterGame()
        {
            Build();

            On.HeroController.Pause += OnPause;
        }

        public override void OnQuitToMenu()
        {
            Root?.Destroy();
            Root = null;
            Titles = new();
            MainButtons = new();
            ExtraButtonPanels = new();

            On.HeroController.Pause -= OnPause;
        }

        public bool Condition()
        {
            return GameManager.instance.IsGamePaused();
        }

        public void Build()
        {
            if (Root == null)
            {
                Root = new(true, $"{GetType().Name} Root");
                Root.VisibilityCondition = Condition;
            }

            MainButtonsGrid = new(Root, "Main Buttons")
            {
                RowDefinitions =
                {
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin)
                },
                ColumnDefinitions =
                {
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin)
                },
                MinHeight = 33f,
                MinWidth = 100f,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new(10f, 865f, 10f, 10f)
            };

            ModToggleText mapToggleText = new();
            mapToggleText.Make();

            Update();
        }

        public static void Update()
        {
            foreach (Title title in Titles)
            {
                title.Update();
            }

            foreach (MainButton mainButton in MainButtons)
            {
                mainButton.Update();
            }

            foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
            {
                ebp.Update();
            }
        }

        private static void OnPause(On.HeroController.orig_Pause orig, HeroController self)
        {
            orig(self);

            foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
            {
                ebp.Hide();
            }

            Update();
        }

        internal static void HideOtherPanels(ExtraButtonPanel visiblePanel)
        {
            foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
            {
                if (ebp != visiblePanel)
                {
                    ebp?.Hide();
                }
            }
        }
    }
}
