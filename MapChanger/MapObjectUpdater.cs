using System.Collections.Generic;
using System.Collections.ObjectModel;
using GlobalEnums;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger;

/// <summary>
/// Updates MapObjects when a map is opened or closed.
/// </summary>
public class MapObjectUpdater : HookModule
{
    private static readonly List<MapObject> _mapObjects = [];

    public static ReadOnlyCollection<MapObject> MapObjects => new(_mapObjects);

    public override void OnEnterGame()
    {
        Events.OnSetGameMap += AddMapPanner;
        Events.OnWorldMap += BeforeOpenWorldMap;
        Events.OnQuickMap += BeforeOpenQuickMap;
        Events.OnCloseMap += BeforeCloseMap;
    }

    public override void OnQuitToMenu()
    {
        foreach (var mapObject in _mapObjects)
        {
            if (mapObject != null)
            {
                mapObject.DestroyAll();
            }
        }

        _mapObjects.Clear();

        Events.OnSetGameMap -= AddMapPanner;
        Events.OnWorldMap -= BeforeOpenWorldMap;
        Events.OnQuickMap -= BeforeOpenQuickMap;
        Events.OnCloseMap -= BeforeCloseMap;
    }

    public static void Add(MapObject mapObject)
    {
        _mapObjects.Add(mapObject);
    }

    private static void AddMapPanner(GameObject goMap)
    {
        var panner = Utils.MakeMonoBehaviour<MapPanner>(goMap, "Map Panner");
        panner.Initialize(goMap);
        Add(panner);
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
        foreach (var mapObject in _mapObjects)
        {
            mapObject.MainUpdate();
        }
    }

    private void ClearNullMapObjects()
    {
        _ = _mapObjects.RemoveAll(mapObject => mapObject == null);
    }
}
