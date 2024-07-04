using System.Collections.Generic;
using GlobalEnums;

namespace MapChanger.UI
{
    /// <summary>
    /// Updates MapUILayers when a map is opened or closed.
    /// </summary>
    public class MapUILayerUpdater : HookModule
    {
        private static readonly List<MapUILayer> mapLayers = [];

        public override void OnEnterGame()
        {
            Add(new GlobalHotkeys());

            Events.OnWorldMap += OnOpenWorldMap;
            Events.OnQuickMap += OnOpenQuickMap;
            Events.OnCloseMap += OnCloseMap;
        }

        public override void OnQuitToMenu()
        {
            Events.OnWorldMap += OnOpenWorldMap;
            Events.OnQuickMap += OnOpenQuickMap;
            Events.OnCloseMap += OnCloseMap;

            RemoveMapLayers();
        }

        public static void Add(MapUILayer layer)
        {
            mapLayers.Add(layer);
            layer.Build();
        }

        internal static void RemoveMapLayers()
        {
            foreach (MapUILayer layer in mapLayers)
            {
                layer.Destroy();
            }

            mapLayers.Clear();
        }

        private static void OnOpenWorldMap(GameMap obj)
        {
            Update();
        }

        private static void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        {
            Update();
        }

        private static void OnCloseMap(GameMap obj)
        {
            Update();
        }

        public static void Update()
        {
            foreach (MapUILayer layer in mapLayers)
            {
                layer.Update();
            }
        }
    }
}
