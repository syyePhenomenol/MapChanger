using Newtonsoft.Json;

namespace MapChanger.Settings;

public class GlobalSettings
{
    private static readonly float[] _scaleFactors = [1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f];

    [JsonProperty]
    public float UIScale { get; private set; } = 1.2f;

    [JsonProperty]
    public bool ShowPauseMenu { get; private set; } = true;

    public void ToggleUIScale()
    {
        for (var i = 0; i < _scaleFactors.Length; i++)
        {
            if (_scaleFactors[i] == UIScale)
            {
                UIScale = _scaleFactors[(i + 1) % _scaleFactors.Length];
                return;
            }
        }

        UIScale = 1f;
    }

    public void ToggleShowPauseMenu()
    {
        ShowPauseMenu = !ShowPauseMenu;
    }
}
