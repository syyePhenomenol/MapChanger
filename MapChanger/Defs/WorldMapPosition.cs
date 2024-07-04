using GlobalEnums;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger.Defs
{
    /// <summary>
    /// Interprets the x and y values of the input MapLocations
    /// as the real world coordinates from the center of the scene, and scaled
    /// correctly to the room on the map.
    /// The first MapLocation that has a MappedScene corresponding to a room sprite is used.
    /// </summary>
    public record WorldMapPosition : MapRoomPosition
    {
        /// <summary>
        /// The unscaled relative x offset of the object from the center of the mapped room.
        /// </summary>
        public float RelativeX { get; private set; }
        /// <summary>
        /// The unscaled relative y offset of the object from the center of the mapped room.
        /// </summary>
        public float RelativeY { get; private set; }

        public WorldMapPosition((string, float, float)[] mapLocations) : base(mapLocations) { }

        public WorldMapPosition(MapLocation[] mapLocations) : base(mapLocations) { }

        private protected override bool TrySetPosition(MapLocation mapLocation)
        {
            if (!BuiltInObjects.TryGetMapRoomPosition(mapLocation.MappedScene, out float baseX, out float baseY)
                || !BuiltInObjects.MappedRooms.TryGetValue(mapLocation.MappedScene, out RoomSprite roomSprite)
                || !Finder.TryGetTileMapDef(mapLocation.MappedScene, out TileMapDef tmd)) return false;

            Vector2 spriteSize = roomSprite.GetComponent<SpriteRenderer>().sprite.bounds.size;
            RelativeX = mapLocation.X / tmd.Width * spriteSize.x - spriteSize.x / 2f;
            RelativeY = mapLocation.Y / tmd.Height * spriteSize.y - spriteSize.y / 2f;
            X = baseX + RelativeX;
            Y = baseY + RelativeY;
            return true;
        }
    }
}
