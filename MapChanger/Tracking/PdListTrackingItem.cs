using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapChanger.Tracking;

public class PdListTrackingItem : TrackingItem
{
    [JsonProperty]
    public string PdListName { get; init; }

    [JsonProperty]
    public string SceneName { get; init; }

    public override bool Has()
    {
        return PlayerData.instance.GetVariable<List<string>>(PdListName).Contains(SceneName);
    }
}
