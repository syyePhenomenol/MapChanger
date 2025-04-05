using Newtonsoft.Json;

namespace MapChanger.Defs;

public enum MiscObjectType
{
    AreaName,
    NextArea,
}

public record MiscObjectDef
{
    [JsonProperty]
    public MiscObjectType Type { get; init; }

    [JsonProperty]
    public ColorSetting ColorSetting { get; init; }
}
