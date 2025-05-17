using Newtonsoft.Json;

namespace MapChanger.Settings;

public class LocalSettings
{
    [JsonProperty]
    public bool Enabled { get; private set; } = false;

    [JsonProperty]
    public string CurrentMod { get; private set; } = nameof(MapChangerMod);

    [JsonProperty]
    public string CurrentModeName { get; private set; } = "Disabled";

    internal void ToggleEnabled()
    {
        Enabled = !Enabled;
    }

    internal void SetCurrentMode(MapMode mode)
    {
        CurrentMod = mode.Mod;
        CurrentModeName = mode.ModeName;
    }
}
