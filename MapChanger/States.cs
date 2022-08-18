using GlobalEnums;

namespace MapChanger
{
    public static class States
    {
        public static bool WorldMapOpen { get; internal set; }
        public static bool QuickMapOpen { get; internal set; }
        public static MapZone CurrentMapZone { get; internal set; }
    }
}
