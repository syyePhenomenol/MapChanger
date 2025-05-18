using System;
using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

public abstract class PauseMenuLayout : UILayout
{
    private readonly StackLayout _fullVerticalStack;
    private readonly PauseMenuTitle _title;
    private readonly StackLayout _horizontalSubstack;
    private readonly DynamicUniformGrid _mainButtonsGrid;
    private readonly StackLayout _extraButtonGridSubstack;

    private readonly List<MainButton> _mainButtons;
    private readonly List<ExtraButtonGrid> _extraButtonGrids;

    public PauseMenuLayout(string mod, string name)
        : base(mod, name)
    {
        _fullVerticalStack = new(this, $"{mod} Full Vertical Stack")
        {
            Padding = new Padding(10f, 750f, 0f, 0f),
            Orientation = Orientation.Vertical,
            Spacing = 10f,
        };

        try
        {
            _title = GetTitle();
            _title.Initialize(this);
            _fullVerticalStack.Children.Add(_title.Text);
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        _horizontalSubstack = new(this, $"{mod} Horizontal Substack")
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10f,
        };
        _fullVerticalStack.Children.Add(_horizontalSubstack);

        _mainButtonsGrid = new(this, $"{mod} Main Buttons")
        {
            Orientation = Orientation.Vertical,
            ChildrenBeforeRollover = 4,
            HorizontalSpacing = 2f,
            VerticalSpacing = 2f,
        };
        _horizontalSubstack.Children.Add(_mainButtonsGrid);

        _extraButtonGridSubstack = new(this, $"{mod} Extra Button Grid Substack")
        {
            Orientation = Orientation.Vertical,
        };
        _horizontalSubstack.Children.Add(_extraButtonGridSubstack);

        _mainButtons = [];
        _extraButtonGrids = [];

        try
        {
            foreach (var button in GetMainButtons())
            {
                button.Initialize(this);
                _mainButtonsGrid.Children.Add(button.Button);
                _mainButtons.Add(button);
            }

            foreach (var grid in GetExtraButtonGrids())
            {
                grid.Initialize(this);
                _extraButtonGridSubstack.Children.Add(grid.Grid);
                _extraButtonGrids.Add(grid);
            }
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        On.HeroController.Pause += OnPause;
        ModeManager.OnModeChanged += CollapseAndUpdate;
    }

    public override void Update()
    {
        try
        {
            _title?.Update();

            foreach (var button in _mainButtons)
            {
                button?.Update();
            }

            foreach (var grids in _extraButtonGrids)
            {
                grids?.Update();
            }
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }
    }

    public override void Destroy()
    {
        On.HeroController.Pause -= OnPause;
        ModeManager.OnModeChanged -= CollapseAndUpdate;

        try
        {
            _title?.Destroy();

            foreach (var button in _mainButtons)
            {
                button?.Destroy();
            }

            foreach (var grid in _extraButtonGrids)
            {
                grid?.Destroy();
            }
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        base.Destroy();
    }

    internal void SetTitleOverrideButton(PauseMenuButton button)
    {
        _title?.SetOverrideButton(button);
    }

    internal void ResetTitle()
    {
        _title?.SetOverrideButton(null);
    }

    internal void ToggleExtraGrid<T>()
        where T : ExtraButtonGrid
    {
        foreach (var grid in _extraButtonGrids)
        {
            if (grid is T)
            {
                grid?.Toggle();
            }
            else
            {
                grid?.Hide();
            }
        }
    }

    internal bool GridOpen<T>()
        where T : ExtraButtonGrid
    {
        foreach (var grid in _extraButtonGrids)
        {
            if (grid is T)
            {
                return grid.Grid.Visibility is Visibility.Visible;
            }
        }

        return false;
    }

    protected override bool ActiveCondition()
    {
        return GameManager.instance.IsGamePaused() && MapChangerMod.GS.ShowPauseMenu;
    }

    protected abstract PauseMenuTitle GetTitle();
    protected abstract IEnumerable<MainButton> GetMainButtons();
    protected abstract IEnumerable<ExtraButtonGrid> GetExtraButtonGrids();

    private void OnPause(On.HeroController.orig_Pause orig, HeroController self)
    {
        orig(self);
        CollapseAndUpdate();
    }

    private void CollapseAndUpdate()
    {
        Collapse();
        Update();
    }

    private void Collapse()
    {
        foreach (var grid in _extraButtonGrids)
        {
            grid?.Hide();
        }
    }
}
