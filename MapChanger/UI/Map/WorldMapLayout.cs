using System;
using System.Collections.Generic;
using GlobalEnums;

namespace MapChanger.UI;

public abstract class WorldMapLayout : MapLayout
{
    private readonly List<WorldMapStack> _stacks;

    public WorldMapLayout(string mod, string name)
        : base(mod, name)
    {
        _stacks = [];

        try
        {
            foreach (var stack in GetStacks())
            {
                stack.Initialize(this);
                _stacks.Add(stack);
            }
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }
    }

    public override void Update()
    {
        try
        {
            foreach (var stack in _stacks)
            {
                stack?.Update();
            }
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }
    }

    public override void Destroy()
    {
        foreach (var stack in _stacks)
        {
            stack?.Destroy();
        }

        base.Destroy();
    }

    protected override bool ActiveCondition()
    {
        return States.WorldMapOpen;
    }

    protected abstract IEnumerable<WorldMapStack> GetStacks();

    protected internal sealed override void OnOpenWorldMap(GameMap obj)
    {
        Update();
    }

    protected internal sealed override void OnOpenQuickMap(GameMap gameMap, MapZone mapZone) { }

    protected internal sealed override void OnCloseMap(GameMap obj) { }
}
