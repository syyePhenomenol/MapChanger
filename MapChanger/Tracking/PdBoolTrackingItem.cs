using Newtonsoft.Json;

namespace MapChanger.Tracking;

public class PdBoolTrackingItem : TrackingItem
{
    [JsonProperty]
    public string PdBoolName { get; init; }

    public override bool Has()
    {
        return PlayerData.instance.GetBool(PdBoolName);
    }
}
