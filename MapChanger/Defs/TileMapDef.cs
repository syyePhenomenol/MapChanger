using Newtonsoft.Json;

namespace MapChanger.Defs
{
    /// <summary>
    /// The real-world width and height of a mapped scene. Used to convert
    /// real-world coordinates to a position on the map.
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
