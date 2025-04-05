using GlobalEnums;

namespace MapChanger.Defs;

public record MappedSceneDef
{
    public string MappedScene { get; init; }
    public MapZone MapZone { get; init; }
}
