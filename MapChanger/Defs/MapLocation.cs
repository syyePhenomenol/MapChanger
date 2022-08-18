using Newtonsoft.Json;

namespace MapChanger.Defs
{
    public record MapLocation
    {
        [JsonProperty]
        public string MappedScene { get; init; }
        [JsonProperty]
        public float X { get; init; }
        [JsonProperty]
        public float Y { get; init; }

        public static implicit operator MapLocation((string, float, float) tuple)
        {
            return new() { MappedScene = tuple.Item1, X = tuple.Item2, Y = tuple.Item3 };
        }

        public static implicit operator (string, float, float)(MapLocation mapLocation)
        {
            return new (mapLocation.MappedScene, mapLocation.X, mapLocation.Y);
        }
    }
}
