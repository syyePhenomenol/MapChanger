using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

namespace MapChanger.Defs
{
    /// <summary>
    /// Interprets the x and y values of the input MapLocations
    /// as the unscaled offset from the center of the Quick Map.
    /// Kingdom's Edge is scaled differently to the other quick maps.
    /// You might need to account for this.
    /// </summary>
    public record QuickMapPosition : IMapPosition
    {
        private const float QUICK_MAP_SCALE = 1.55f;

        public float X { get; protected private set; }
        public float Y { get; protected private set; }

        public QuickMapPosition((float x, float y) offset)
        {
            SetPosition(new(offset.x, offset.y));
        }

        public QuickMapPosition(Vector2 offset)
        {
            SetPosition(offset);
        }

        private void SetPosition(Vector2 offset)
        {
            //float scaleCorrection = States.CurrentMapZone is MapZone.OUTSKIRTS ? 0.935f : 1.0f;

            if (mapZoneOffsets.TryGetValue(States.CurrentMapZone, out Vector2 mapZoneOffset))
            {
                X = offset.x - mapZoneOffset.x / QUICK_MAP_SCALE;
                Y = offset.y - mapZoneOffset.y / QUICK_MAP_SCALE;
            }
        }

        private static readonly Dictionary<MapZone, Vector2> mapZoneOffsets = new()
        {
            { MapZone.CROSSROADS, new(-0.04f, -7.88f) },
            { MapZone.WATERWAYS, new(-8.1f, 7.4f) },
            { MapZone.CLIFFS, new(13.89f, -14.17f) },
            { MapZone.OUTSKIRTS, new(-21.9f, 4.0f) },
            { MapZone.GREEN_PATH, new(16.31f, -7.87f) },
            { MapZone.FOG_CANYON, new(11.3f, -3.3f) },
            { MapZone.WASTES, new(5.05f, 0.47f) },
            { MapZone.ROYAL_GARDENS, new(19.7f, -0.3f) },
            { MapZone.DEEPNEST, new(16.0f, 6.7f) },
            { MapZone.TOWN, new(4.07f, -11.62f) },
            { MapZone.RESTING_GROUNDS, new(-14.6f, -7.0f) },
            { MapZone.MINES, new(-9.17f, -12.8f) },
            { MapZone.ABYSS, new(-8.3f, 14.6f) },
            { MapZone.CITY, new(-11.98f, 0.65f) },
            { MapZone.WHITE_PALACE, new(3.07f, -23.0f) },
            { MapZone.GODS_GLORY, new(-8.5f, -22.0f) }
        };
    }
}
