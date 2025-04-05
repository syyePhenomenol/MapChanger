using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public class PauseMenu : HookModule
{
    private static LayoutRoot _root;
    private static GridLayout _mainButtonsGrid;
    private static List<Title> _titles;
    private static List<MainButton> _mainButtons;
    private static List<ExtraButtonPanel> _extraButtonPanels;

    public override void OnEnterGame()
    {
        Build();

        On.HeroController.Pause += OnPause;
    }

    public override void OnQuitToMenu()
    {
        _root?.Destroy();
        _root = null;
        _mainButtonsGrid = null;
        _titles = null;
        _mainButtons = null;
        _extraButtonPanels = null;

        On.HeroController.Pause -= OnPause;
    }

    public static void Add(Title title)
    {
        title.Initialize(_root);
        _titles.Add(title);
    }

    public static void Add(MainButton mb)
    {
        mb.Initialize(_root);
        _mainButtonsGrid.Children.Add(mb.Button);
        _mainButtons.Add(mb);
    }

    public static void Add(ExtraButtonPanel ebp)
    {
        ebp.Initialize(_root);
        _extraButtonPanels.Add(ebp);
    }

    public static void Update()
    {
        foreach (var title in _titles)
        {
            title.Update();
        }

        foreach (var mainButton in _mainButtons)
        {
            mainButton.Update();
        }

        foreach (var ebp in _extraButtonPanels)
        {
            ebp.Update();
        }
    }

    private void Build()
    {
        _root ??= new(true, $"{GetType().Name} Root") { VisibilityCondition = GameManager.instance.IsGamePaused };

        _mainButtonsGrid = new(_root, "Main Buttons")
        {
            RowDefinitions =
            {
                new GridDimension(1, GridUnit.AbsoluteMin),
                new GridDimension(1, GridUnit.AbsoluteMin),
                new GridDimension(1, GridUnit.AbsoluteMin),
            },
            ColumnDefinitions =
            {
                new GridDimension(1, GridUnit.AbsoluteMin),
                new GridDimension(1, GridUnit.AbsoluteMin),
                new GridDimension(1, GridUnit.AbsoluteMin),
                new GridDimension(1, GridUnit.AbsoluteMin),
            },
            MinHeight = 33f,
            MinWidth = 100f,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Padding = new(10f, 865f, 10f, 10f),
        };

        _titles = [];
        _mainButtons = [];
        _extraButtonPanels = [];

        ModToggleText mapToggleText = new();
        mapToggleText.Initialize(_root);
        _titles.Add(mapToggleText);

        Update();
    }

    private static void OnPause(On.HeroController.orig_Pause orig, HeroController self)
    {
        orig(self);

        foreach (var ebp in _extraButtonPanels)
        {
            ebp.Hide();
        }

        Update();
    }

    internal static void HideOtherPanels(ExtraButtonPanel visiblePanel)
    {
        foreach (var ebp in _extraButtonPanels)
        {
            if (ebp != visiblePanel)
            {
                ebp?.Hide();
            }
        }
    }
}
