using Newtonsoft.Json;

namespace MapChanger.Tracking
{
    public class PdIntTrackingItem : TrackingItem
    {
        [JsonProperty]
        public string PdIntName { get; init; }

        [JsonProperty]
        public int PdIntValue { get; init; }

        public override bool Has()
        {
            return PlayerData.instance.GetInt(PdIntName) >= PdIntValue;
        }
    }
}