using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using MapChanger.Defs;

namespace MapChanger;

public static class Finder
{
    private static Dictionary<string, MappedSceneDef> _mappedScenes;
    private static Dictionary<string, TileMapDef> _tileMaps;
    private static HashSet<string> _minimalMapScenes;
    private static Dictionary<string, MapLocationDef> _locations;
    private static readonly Dictionary<string, MapLocationDef> _injectedLocations = [];

    internal static void Load()
    {
        _mappedScenes = JsonUtil.Deserialize<Dictionary<string, MappedSceneDef>>(
            "MapChanger.Resources.mappedScenes.json"
        );
        _tileMaps = JsonUtil.Deserialize<Dictionary<string, TileMapDef>>("MapChanger.Resources.tileMaps.json");
        _minimalMapScenes = JsonUtil.Deserialize<HashSet<string>>("MapChanger.Resources.minimalMap.json");
        _locations = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapChanger.Resources.locations.json");

        if (Dependencies.HasAdditionalMaps)
        {
            var mappedSceneLookupAM = JsonUtil.Deserialize<Dictionary<string, MappedSceneDef>>(
                "MapChanger.Resources.mappedScenesAM.json"
            );
            foreach ((var scene, var msd) in mappedSceneLookupAM.Select(kvp => (kvp.Key, kvp.Value)))
            {
                _mappedScenes[scene] = msd;
            }
        }
    }

    public static void InjectLocations(Dictionary<string, MapLocationDef> locations)
    {
        foreach ((var name, var mpd) in locations.Select(kvp => (kvp.Key, kvp.Value)))
        {
            _injectedLocations[name] = mpd;
        }
    }

    public static bool TryGetLocation(string name, out MapLocationDef mld)
    {
        mld = null;
        if (name is null)
        {
            return false;
        }

        if (_injectedLocations.TryGetValue(name, out mld))
        {
            return true;
        }

        if (_locations.TryGetValue(name, out mld))
        {
            return true;
        }

        return false;
    }

    public static Dictionary<string, MapLocationDef> GetAllVanillaLocations()
    {
        return _locations;
    }

    public static Dictionary<string, MapLocationDef> GetAllLocations()
    {
        Dictionary<string, MapLocationDef> newLocations = new(_locations);
        foreach ((var name, var mld) in _injectedLocations.Select(kvp => (kvp.Key, kvp.Value)))
        {
            newLocations[name] = mld;
        }

        return newLocations;
    }

    public static bool IsMinimalMapScene(string scene)
    {
        return _minimalMapScenes.Contains(scene);
    }

    public static string GetMappedScene(string scene)
    {
        if (scene is null)
        {
            return default;
        }

        if (!_mappedScenes.TryGetValue(scene, out var msd))
        {
            return default;
        }

        return msd.MappedScene;
    }

    public static MapZone GetMapZone(string scene)
    {
        if (scene is null)
        {
            return default;
        }

        if (!_mappedScenes.TryGetValue(scene, out var msd))
        {
            return default;
        }

        return msd.MapZone;
    }

    public static bool IsScene(string scene)
    {
        if (scene is null)
        {
            return false;
        }

        return _mappedScenes.ContainsKey(scene);
    }

    public static bool IsMappedScene(string scene)
    {
        if (scene is null)
        {
            return false;
        }

        if (_mappedScenes.TryGetValue(scene, out var msd))
        {
            return scene == msd.MappedScene;
        }

        return false;
    }

    public static MapZone GetCurrentMapZone()
    {
        return GetMapZone(Utils.CurrentScene());
    }

    public static bool TryGetTileMapDef(string scene, out TileMapDef tmd)
    {
        tmd = null;
        if (scene is null)
        {
            return false;
        }

        return _tileMaps.TryGetValue(scene, out tmd);
    }
}
