using Newtonsoft.Json;

namespace MapChanger.Tracking
{
    public abstract class TrackingItem
    {
        [JsonProperty]
        public string Name { get; init; }
        
        public abstract bool Has();
    }
}