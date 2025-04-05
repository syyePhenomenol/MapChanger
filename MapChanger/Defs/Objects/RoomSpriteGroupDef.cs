using Newtonsoft.Json;

namespace MapChanger.Defs;

public record RoomSpriteGroupDef
{
    [JsonProperty]
    public string[] RoomSprites { get; init; }

    [JsonProperty]
    public ColorSetting ColorSetting { get; init; }
}
