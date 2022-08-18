using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapChanger
{
    public static class Dependencies
    {
        private const string MAGIC_UI = "MagicUI";
        private const string VASI = "Vasi";
        private const string ADDITIONAL_MAPS = "AdditionalMaps";

        public static Dictionary<string, Assembly> strictDependencies = new()
        {
            { MAGIC_UI, null },
            { VASI, null }
        };

        public static Dictionary<string, Assembly> optionalDependencies = new()
        {
            { ADDITIONAL_MAPS, null },
        };

        public static void GetDependencies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (strictDependencies.ContainsKey(assembly.GetName().Name))
                {
                    strictDependencies[assembly.GetName().Name] = assembly;
                }
                if (optionalDependencies.ContainsKey(assembly.GetName().Name))
                {
                    optionalDependencies[assembly.GetName().Name] = assembly;
                }
            }
        }

        public static bool HasAdditionalMaps()
        {
            return optionalDependencies[ADDITIONAL_MAPS] is not null;
        }
    }
}
