using System.Collections.Generic;
using System.Collections.ObjectModel;
using GlobalEnums;
using MapChanger.MonoBehaviours;

namespace MapChanger
{
    /// <summary>
    /// Updates MapObjects when a map is opened or closed.
    /// </summary>
    public class MapObjectUpdater : HookModule
    {
        private static readonly List<MapObject> mapObjects = new();
        public static ReadOnlyCollection<MapObject> MapObjects => mapObjects.AsReadOnly();

        public override void OnEnterGame()
        {
            Events.OnWorldMap += BeforeOpenWorldMap;
            Events.OnQuickMap += BeforeOpenQuickMap;
            Events.OnCloseMap += BeforeCloseMap;
        }

        public override void OnQuitToMenu()
        {
            foreach (MapObject mapObject in mapObjects)
            {
                if (mapObject != null)
                {
                    mapObject.DestroyAll();
                }
            }

            mapObjects.Clear();

            Events.OnWorldMap -= BeforeOpenWorldMap;
            Events.OnQuickMap -= BeforeOpenQuickMap;
            Events.OnCloseMap -= BeforeCloseMap;
        }

        public static void Add(MapObject mapObject)
        {
            mapObjects.Add(mapObject);
        }

        private void BeforeOpenWorldMap(GameMap obj)
        {
            ClearNullMapObjects();
            Update();
        }

        private void BeforeOpenQuickMap(GameMap arg1, MapZone arg2)
        {
            ClearNullMapObjects();
            Update();
        }

        private void BeforeCloseMap(GameMap obj)
        {
            ClearNullMapObjects();
            Update();
        }

        internal static void Update()
        {
            foreach (MapObject mapObject in mapObjects)
            {
                mapObject.MainUpdate();
            }
        }

        private void ClearNullMapObjects()
        {
            mapObjects.RemoveAll(mapObject => mapObject == null);
        }
    }
}
