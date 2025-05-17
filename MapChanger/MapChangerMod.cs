using MapChanger.Input;
using MapChanger.Settings;
using Modding;

namespace MapChanger;

public class MapChangerMod : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings>
{
    public static MapChangerMod Instance { get; private set; }
    public static GlobalSettings GS { get; private set; } = new();
    public static LocalSettings LS { get; private set; } = new();

    public override string GetVersion()
    {
        return "1.3.4";
    }

    public void OnLoadGlobal(GlobalSettings s)
    {
        GS = s;
    }

    public GlobalSettings OnSaveGlobal()
    {
        return GS;
    }

    public void OnLoadLocal(LocalSettings ls)
    {
        LS = ls;
    }

    public LocalSettings OnSaveLocal()
    {
        return LS;
    }

    public override void Initialize()
    {
        Instance = this;

        Dependencies.FindDependencies();

        if (!Dependencies.HasMagicUI)
        {
            Log("MagicUI is not installed. MapChangerMod disabled");
            return;
        }

        if (!Dependencies.HasVasi)
        {
            Log("Vasi is not installed. MapChangerMod disabled");
            return;
        }

        Finder.Load();

        InputManager.Add(new ModEnabledInput());
        InputManager.Add(new ToggleModeInput());

        Events.Initialize();
    }

    public static bool IsEnabled()
    {
        return LS.Enabled;
    }

    public static void ResetGlobalSettings()
    {
        GS = new();
    }
}
