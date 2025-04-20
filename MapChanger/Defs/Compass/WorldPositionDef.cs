using Newtonsoft.Json;

namespace MapChanger.Defs;

public record WorldPositionDef
{
    [JsonProperty]
    public float X { get; init; }

    [JsonProperty]
    public float Y { get; init; }

    public static implicit operator (float x, float y)(WorldPositionDef wpd) => (wpd.X, wpd.Y);

    public static implicit operator WorldPositionDef((float x, float y) tuple) => new() { X = tuple.x, Y = tuple.y };
}
