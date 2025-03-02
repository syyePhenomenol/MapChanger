using System.Collections.Generic;
using System.Reflection;
using MapChanger.Map;
using Modding;

namespace MapChanger
{
    public class MapChangerMod : Mod, ILocalSettings<Settings>
    {
        public static MapChangerMod Instance;
        public override string GetVersion() => "1.2.2";
        public void OnLoadLocal(Settings ls) => Settings.Instance = ls;
        public Settings OnSaveLocal() => Settings.Instance;

        public override void Initialize()
        {
            Instance = this;

            Dependencies.GetDependencies();
            if (!Dependencies.HasMagicUI)
            {
                Log($"MagicUI is not installed. MapChangerMod disabled");
                return;
            }
            if (!Dependencies.HasVasi)
            {
                Log($"Vasi is not installed. MapChangerMod disabled");
                return;
            }

            Finder.Load();
            Tracker.Load();
            BuiltInObjects.Load();
            VariableOverrides.Load();

            Events.Initialize();
        }
    }
}
