using Newtonsoft.Json;

namespace MapChanger.Defs;

public record MapLocationDef
{
    /// <summary>
    /// Must be unique.
    /// </summary>
    [JsonProperty]
    public string Name { get; init; }

    /// <summary>
    /// In some situations, this can be null.
    /// </summary>
    [JsonProperty]
    public string SceneName { get; init; }

    /// <summary>
    /// Represents the possible positions the object can be at (if the MappedScene exists). The elements are ordered by preference.
    /// </summary>
    [JsonProperty]
    public MapLocation[] MapLocations { get; init; }

    /// <summary>
    /// Coordinates of the location in the world. Can be null.
    /// </summary>
    [JsonProperty]
    public WorldPositionDef WorldPosition { get; init; }
}
