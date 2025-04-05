using System;

namespace MapChanger;

public static class Dependencies
{
    public static bool HasMagicUI { get; private set; } = false;
    public static bool HasVasi { get; private set; } = false;
    public static bool HasAdditionalMaps { get; private set; } = false;
    public static bool HasDebugMod { get; private set; } = false;

    public static void FindDependencies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            switch (assembly.GetName().Name)
            {
                case "MagicUI":
                    HasMagicUI = true;
                    continue;
                case "Vasi":
                    HasVasi = true;
                    continue;
                case "AdditionalMaps":
                    HasAdditionalMaps = true;
                    continue;
                case "DebugMod":
                    HasDebugMod = true;
                    continue;
                default:
                    continue;
            }
        }
    }
}
