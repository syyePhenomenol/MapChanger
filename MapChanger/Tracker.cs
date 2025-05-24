using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using MapChanger.Tracking;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;

namespace MapChanger;

public class Tracker : HookModule
{
    private static Dictionary<string, TrackingItem> _trackingItems;
    private static HashSet<(string, string)> _obtainedSceneData;
    private static HashSet<string> _scenesVisited;

    internal static List<string> ScenesVisitedList => [.. _scenesVisited];

    public override void OnEnterGame()
    {
        GetPreviouslyObtainedItems();

        On.PlayMakerFSM.OnEnable += AddItemTrackers;
        On.HealthManager.SendDeathEvent += TrackEnemy;
        On.GeoRock.SetMyID += RenameDupeGeoRockIds;

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += AddSceneVisited;
    }

    public override void OnQuitToMenu()
    {
        _trackingItems = null;
        _scenesVisited = null;
        _obtainedSceneData = null;

        On.PlayMakerFSM.OnEnable -= AddItemTrackers;
        On.HealthManager.SendDeathEvent -= TrackEnemy;
        On.GeoRock.SetMyID -= RenameDupeGeoRockIds;

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= AddSceneVisited;
    }

    public static void AddSceneVisited(string scene)
    {
        _scenesVisited.Add(scene);
    }

    public static bool HasClearedLocation(string name)
    {
        if (_trackingItems.TryGetValue(name, out var ti))
        {
            return ti.Has();
        }

        return false;
    }

    public static bool HasVisitedScene(string scene)
    {
        return _scenesVisited.Contains(scene);
    }

    public static bool HasSceneData(string id, string sceneName)
    {
        return _obtainedSceneData.Contains((id, sceneName));
    }

    internal static void VerifyTrackingDefs()
    {
        foreach (var name in _trackingItems.Keys)
        {
            MapChangerMod.Instance.LogDebug($"Has {name}: {HasClearedLocation(name)}");
        }
    }

    private static void GetPreviouslyObtainedItems()
    {
        _trackingItems = JsonUtil.Deserialize<Dictionary<string, TrackingItem>>(
            "MapChanger.Resources.trackingItems.json"
        );

        _scenesVisited = new(PlayerData.instance.scenesVisited);
        _obtainedSceneData = [];

        foreach (var grd in GameManager.instance.sceneData.geoRocks)
        {
            if (grd.hitsLeft <= 0)
            {
                _ = _obtainedSceneData.Add((grd.id, grd.sceneName));
            }
        }

        HashSet<(string, string)> persistentBoolItems =
            new(_trackingItems.Values.Where(i => i is SdTrackingItem).Select(i => ((SdTrackingItem)i).SceneDataPair));

        persistentBoolItems.UnionWith(
            _trackingItems
                .Values.Where(i => i is MultiTrackingItem)
                .SelectMany(i => ((MultiTrackingItem)i).GetSdTrackingItemPairs())
        );

        foreach (var pbd in GameManager.instance.sceneData.persistentBoolItems)
        {
            var pair = (pbd.id, pbd.sceneName);
            if (persistentBoolItems.Contains(pair) && pbd.activated)
            {
                _ = _obtainedSceneData.Add(pair);
            }
        }
    }

    private static void TrackEnemy(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
    {
        orig(self);

        var scene = Utils.CurrentScene();

        switch (self.gameObject.name)
        {
            case "Egg Sac":
                if (scene is "Deepnest_East_14")
                {
                    _ = _obtainedSceneData.Add((self.gameObject.name, scene));
                }

                break;
            case "Grub Mimic":
            case "Grub Mimic 1":
            case "Grub Mimic 2":
            case "Grub Mimic 3":
            case "Mega Zombie Beam Miner (1)":
            case "Zombie Beam Miner Rematch":
                _ = _obtainedSceneData.Add((self.gameObject.name, scene ?? ""));
                break;
            case "Mage Knight":
                if (scene is "Ruins1_23")
                {
                    _ = _obtainedSceneData.Add(("Battle Scene v2", scene));
                }
                else if (scene is "Ruins1_31b")
                {
                    _ = _obtainedSceneData.Add(("Battle Scene v2", scene));
                }

                break;
            case "Giant Fly":
                if (scene is "Crossroads_04")
                {
                    _ = _obtainedSceneData.Add(("Battle Scene", scene));
                }

                break;
            default:
                break;
        }
    }

    private static void AddItemTrackers(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        var name = self.gameObject.name;
        FsmState state;

        if (self.FsmName is "Shiny Control")
        {
            if (!FsmUtil.TryGetState(self, "Finish", out state))
                return;
        }
        else if (name is "Heart Piece" or "Vessel Fragment")
        {
            if (!FsmUtil.TryGetState(self, "Get", out state))
                return;
        }
        else if (self.FsmName is "Chest Control")
        {
            if (!FsmUtil.TryGetState(self, "Open", out state))
                return;
        }
        else if (self.FsmName is "Geo Rock")
        {
            if (FsmUtil.TryGetState(self, "Destroy", out state))
            {
                FsmUtil.AddAction(state, new TrackGeoRock(self.gameObject));
                return;
            }

            return;
        }
        else
        {
            return;
        }

        FsmUtil.AddAction(state, new TrackItem(name));
    }

    private static void RenameDupeGeoRockIds(On.GeoRock.orig_SetMyID orig, GeoRock self)
    {
        orig(self);

        if (
            self.gameObject.scene.name is "Crossroads_ShamanTemple"
            && self.gameObject.name is "Geo Rock 2"
            && self.transform.parent != null
        )
        {
            self.geoRockData.id = "_Items/Geo Rock 2";
        }

        if (
            self.gameObject.scene.name is "Abyss_06_Core"
            && self.gameObject.name is "Geo Rock Abyss"
            && self.transform.parent != null
        )
        {
            self.geoRockData.id = "_Props/Geo Rock Abyss";
        }
    }

    private static void AddSceneVisited(Scene from, Scene to)
    {
        AddSceneVisited(to.name);
    }

    private class TrackGeoRock : FsmStateAction
    {
        private readonly GameObject _go;
        private readonly GeoRockData _grd;

        internal TrackGeoRock(GameObject go)
        {
            _go = go;
            _grd = _go.GetComponent<GeoRock>().geoRockData;
        }

        public override void OnEnter()
        {
            if (_grd.id.Contains("-"))
            {
                return;
            }

            _ = _obtainedSceneData.Add((_grd.id, _grd.sceneName));
            MapChangerMod.Instance.LogDebug("Geo Rock broken");
            MapChangerMod.Instance.LogDebug(" ID: " + _grd.id);
            MapChangerMod.Instance.LogDebug(" Scene: " + _grd.sceneName);

            Finish();
        }
    }

    private class TrackItem : FsmStateAction
    {
        private readonly string _name;

        internal TrackItem(string name)
        {
            this._name = name;
        }

        public override void OnEnter()
        {
            if (_name.Contains("-"))
            {
                return;
            }

            var scene = Utils.CurrentScene() ?? "";

            _ = _obtainedSceneData.Add((_name, scene));

            MapChangerMod.Instance.LogDebug("Item picked up");
            MapChangerMod.Instance.LogDebug(" Name: " + _name);
            MapChangerMod.Instance.LogDebug(" Scene: " + scene);

            Finish();
        }
    }
}
