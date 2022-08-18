using GlobalEnums;
using MapChanger.Map;

namespace MapChanger.Defs
{
    /// <summary>
    /// Interprets the x and y values of the input MapLocations
    /// as the unscaled offset from the center of the mapped room.
    /// The first MapLocation that has a MappedScene corresponding to a room sprite is used.
    /// </summary>
    public record MapRoomPosition : IMapPosition
    {
        public float X { get; protected private set; }
        public float Y { get; protected private set; }
        public string MappedScene { get; private set; }
        public MapZone MapZone { get; private set; }

        public MapRoomPosition((string, float, float)[] mapLocations)
        {
            foreach (MapLocation mapLocation in mapLocations)
            {
                if (TrySetPosition(mapLocation))
                {
                    SetMappedScene(mapLocation);
                    break;
                }
            }
        }

        public MapRoomPosition(MapLocation[] mapLocations)
        {
            foreach (MapLocation mapLocation in mapLocations)
            {
                if (TrySetPosition(mapLocation))
                {
                    SetMappedScene(mapLocation);
                    break;
                }
            }
        }

        protected private virtual bool TrySetPosition(MapLocation mapLocation)
        {
            if (!BuiltInObjects.TryGetMapRoomPosition(mapLocation.MappedScene, out float baseX, out float baseY)) return false;
            X = baseX + mapLocation.X;
            Y = baseY + mapLocation.Y;
            return true;
        }

        private void SetMappedScene(MapLocation mapLocation)
        {
            MappedScene = mapLocation.MappedScene;
            MapZone = Finder.GetMapZone(mapLocation.MappedScene);
        }
    }
}
