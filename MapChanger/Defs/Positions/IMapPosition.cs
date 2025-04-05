namespace MapChanger.Defs;

public interface IMapPosition
{
    /// <summary>
    /// The X offset relative to the center of the GameMap transform.
    /// </summary>
    float X { get; }

    /// <summary>
    /// The Y offset relative to the center of the GameMap transform.
    /// </summary>
    float Y { get; }
}
