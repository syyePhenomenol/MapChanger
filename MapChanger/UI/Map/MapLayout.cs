using GlobalEnums;

namespace MapChanger.UI;

public abstract class MapLayout : UILayout
{
    public MapLayout(string mod, string name)
        : base(mod, name)
    {
        Events.OnWorldMap += OnOpenWorldMap;
        Events.OnQuickMap += OnOpenQuickMap;
        Events.OnCloseMap += OnCloseMap;
    }

    public override void Destroy()
    {
        Events.OnWorldMap -= OnOpenWorldMap;
        Events.OnQuickMap -= OnOpenQuickMap;
        Events.OnCloseMap -= OnCloseMap;

        base.Destroy();
    }

    protected internal abstract void OnOpenWorldMap(GameMap obj);
    protected internal abstract void OnOpenQuickMap(GameMap gameMap, MapZone mapZone);
    protected internal abstract void OnCloseMap(GameMap obj);
}
