namespace MapChanger.Defs;

/// <summary>
/// Interprets the x and y values of the input tuple
/// as the unscaled offset from the center of the entire map.
/// </summary>
public record AbsMapPosition : IMapPosition
{
    public float X { get; init; }
    public float Y { get; init; }

    public AbsMapPosition((float x, float y) offset)
    {
        X = offset.x;
        Y = offset.y;
    }

    public AbsMapPosition(MapLocation mapLocation)
    {
        X = mapLocation.X;
        Y = mapLocation.Y;
    }
}
