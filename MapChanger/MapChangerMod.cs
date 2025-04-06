using MapChanger.Input;
using Modding;

namespace MapChanger;

public class MapChangerMod : Mod, ILocalSettings<Settings>
{
    public static MapChangerMod Instance { get; private set; }

    public override string GetVersion()
    {
        return "1.2.2";
    }

    public void OnLoadLocal(Settings ls)
    {
        Settings.Instance = ls;
    }

    public Settings OnSaveLocal()
    {
        return Settings.Instance;
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
}
