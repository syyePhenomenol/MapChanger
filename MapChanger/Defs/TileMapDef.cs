using Newtonsoft.Json;

namespace MapChanger.Defs
{
    /// <summary>
    /// Used to convert real-world coordinates to a position on the map.
    /// </summary>
    public record TileMapDef
    {
        [JsonProperty]
        public string SceneName { get; init; }
        [JsonProperty]
        public int Width { get; init; }
        [JsonProperty]
        public int Height { get; init; }
    }
}
