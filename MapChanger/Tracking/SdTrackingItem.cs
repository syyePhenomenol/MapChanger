using Newtonsoft.Json;

namespace MapChanger.Tracking;

public class SdTrackingItem : TrackingItem
{
    [JsonProperty]
    public string Id { get; init; }

    [JsonProperty]
    public string SceneName { get; init; }

    public (string, string) SceneDataPair => (Id, SceneName);

    public override bool Has()
    {
        return Tracker.HasSceneData(Id, SceneName);
    }
}
