using System.Collections.Generic;
using GlobalEnums;
using MapChanger.Map;

namespace MapChanger.Defs;

/// <summary>
/// Interprets the x and y values of the input MapLocations
/// as the unscaled offset from the center of the mapped room.
/// The first MapLocation that has a MappedScene corresponding to a room sprite is used.
/// </summary>
public record MapRoomPosition : IMapPosition
{
    public MapRoomPosition(IEnumerable<MapLocation> mapLocations)
    {
        foreach (var mapLocation in mapLocations)
        {
            if (TrySetPosition(mapLocation))
            {
                SetMappedScene(mapLocation);
                break;
            }
        }
    }

    /// <summary>
    /// Assumes the MapLocation provided has a valid MappedScene.
    /// </summary>
    /// <param name="mapLocation"></param>
    public MapRoomPosition(MapLocation mapLocation)
    {
        _ = TrySetPosition(mapLocation);
        SetMappedScene(mapLocation);
    }

    public float X { get; private protected set; }
    public float Y { get; private protected set; }
    public string MappedScene { get; private set; }
    public MapZone MapZone { get; private set; }

    private protected virtual bool TrySetPosition(MapLocation mapLocation)
    {
        if (!BuiltInObjects.TryGetMapRoomPosition(mapLocation.MappedScene, out var baseX, out var baseY))
        {
            return false;
        }

        X = baseX + mapLocation.X;
        Y = baseY + mapLocation.Y;
        return true;
    }

    private void SetMappedScene(MapLocation mapLocation)
    {
        MappedScene = mapLocation.MappedScene;
        MapZone = Finder.GetMapZone(mapLocation.MappedScene);
    }
}
