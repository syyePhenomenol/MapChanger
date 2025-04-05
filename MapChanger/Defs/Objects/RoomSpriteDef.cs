using GlobalEnums;
using Newtonsoft.Json;

namespace MapChanger.Defs;

public record RoomSpriteDef
{
    [JsonProperty]
    public ColorSetting ColorSetting { get; init; }

    [JsonProperty]
    public string SceneName { get; init; }

    [JsonIgnore]
    public MapZone MapZone => Finder.GetMapZone(SceneName);
}
