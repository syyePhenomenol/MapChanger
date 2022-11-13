using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using MapChanger.Defs;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;

namespace MapChanger
{
    public class Tracker : HookModule
    {
        internal record TrackingDef
        {
            [JsonProperty]
            internal string ObjectName { get; init; }
            [JsonProperty]
            internal string PdBoolName { get; init; }
            [JsonProperty]
            internal string[] PdBoolNames { get; init; }
            [JsonProperty]
            internal string PdIntName { get; init; }
            [JsonProperty]
            internal int PdIntValue { get; init; }
            [JsonProperty]
            internal string PdListName { get; init; }
        }

        private class TrackGeoRock : FsmStateAction
        {
            private readonly GameObject go;
            private readonly GeoRockData grd;

            internal TrackGeoRock(GameObject go)
            {
                this.go = go;
                grd = this.go.GetComponent<GeoRock>().geoRockData;
            }

            public override void OnEnter()
            {
                if (grd.id.Contains("-")) return;
                AddVanillaItem(grd.id, grd.sceneName);
                MapChangerMod.Instance.LogDebug("Geo Rock broken");
                MapChangerMod.Instance.LogDebug(" ID: " + grd.id);
                MapChangerMod.Instance.LogDebug(" Scene: " + grd.sceneName);

                Finish();
            }
        }

        private class TrackItem : FsmStateAction
        {
            private readonly string name;

            internal TrackItem(string name)
            {
                this.name = name;
            }

            public override void OnEnter()
            {
                if (name.Contains("-")) return;
                string scene = Utils.CurrentScene() ?? "";
                AddVanillaItem(name, scene);
                MapChangerMod.Instance.LogDebug("Item picked up");
                MapChangerMod.Instance.LogDebug(" Name: " + name);
                MapChangerMod.Instance.LogDebug(" Scene: " + scene);

                Finish();
            }
        }

        private static Dictionary<string, TrackingDef> trackingDefs = new();

        private static HashSet<string> clearedLocations;

        public static HashSet<string> ScenesVisited = new();

        internal static void Load()
        {
            trackingDefs = JsonUtil.Deserialize<Dictionary<string, TrackingDef>>("MapChanger.Resources.tracking.json");
        }

        internal static void VerifyTrackingDefs()
        {
            foreach (string name in trackingDefs.Keys)
            {
                MapChangerMod.Instance.LogDebug($"Has {name}: {HasClearedLocation(name)}");
            }
        }

        public override void OnEnterGame()
        {
            ScenesVisited = new(PlayerData.instance.scenesVisited);
            GetPreviouslyObtainedItems();

            On.PlayMakerFSM.OnEnable += AddItemTrackers;
            On.HealthManager.SendDeathEvent += TrackEnemy;
            On.GeoRock.SetMyID += RenameDupeGeoRockIds;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += AddSceneVisited;
        }

        public override void OnQuitToMenu()
        {
            On.PlayMakerFSM.OnEnable -= AddItemTrackers;
            On.HealthManager.SendDeathEvent -= TrackEnemy;
            On.GeoRock.SetMyID -= RenameDupeGeoRockIds;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= AddSceneVisited;
        }

        public static bool HasClearedLocation(string name)
        {
            if (trackingDefs.TryGetValue(name, out TrackingDef td))
            {
                if (td.PdBoolName is not null)
                {
                    return PlayerData.instance.GetBool(td.PdBoolName);
                }
                if (td.PdBoolNames is not null)
                {
                    return td.PdBoolNames.All(boolName => PlayerData.instance.GetBool(boolName));
                }
                if (td.PdIntName is not null)
                {
                    return PlayerData.instance.GetInt(td.PdIntName) >= td.PdIntValue;
                }
                if (Finder.TryGetLocation(name, out MapLocationDef mld))
                {
                    if (td.ObjectName is not null)
                    {
                        return HasObtainedItem(td.ObjectName, mld.SceneName);
                    }
                    if (td.PdListName is not null)
                    {
                        return PlayerData.instance.GetVariable<List<string>>(td.PdListName).Contains(mld.SceneName);
                    }
                }
                MapChangerMod.Instance.LogWarn($"All members of TrackingDef are null! {name}");
            }
            return false;
        }

        public static bool HasObtainedItem(string objectName, string scene)
        {
            return clearedLocations.Contains($"{objectName}-{scene}");
        }

        public static bool HasVisitedScene(string scene)
        {
            return ScenesVisited.Contains(scene);
        }

        private static void GetPreviouslyObtainedItems()
        {
            clearedLocations = new();

            foreach (GeoRockData grd in GameManager.instance.sceneData.geoRocks)
            {
                if (grd.hitsLeft > 0) continue;
                AddVanillaItem(grd.id, grd.sceneName);
            }
            foreach (PersistentBoolData pbd in GameManager.instance.sceneData.persistentBoolItems)
            {
                // Ignore SceneData added by ItemChanger
                if (!pbd.activated || pbd.id.Contains("-")) continue;

                if ((pbd.id.Contains("Shiny Item")
                    || pbd.id is "Heart Piece"
                    || pbd.id is "Vessel Fragment"
                    || pbd.id.Contains("Chest")
                    || pbd.id is "Grub Mimic"
                    || pbd.id is "Grub Mimic 1"
                    || pbd.id is "Grub Mimic 2"
                    || pbd.id is "Grub Mimic 3"
                    // Crystal/Enraged Guardian Boss Geo
                    || pbd.id is "Mega Zombie Beam Miner (1)"
                    || pbd.id is "Zombie Beam Miner Rematch"))
                {
                    AddVanillaItem(pbd.id, pbd.sceneName);
                }
                // Soul Warrior Sanctum Boss Geo
                else if (pbd.id is "Battle Scene v2" && pbd.sceneName is "Ruins1_23")
                {
                    AddVanillaItem("Mage Knight", pbd.sceneName);
                }
                // Soul Warrior Elegant Key Boss Geo
                else if (pbd.id is "Battle Scene v2" && pbd.sceneName is "Ruins1_31")
                {
                    AddVanillaItem("Mage Knight", $"{pbd.sceneName}b");
                }
                // Gruz Mother Boss Geo
                else if (pbd.id is "Battle Scene" && pbd.sceneName is "Crossroads_04")
                {
                    AddVanillaItem("Giant Fly", pbd.sceneName);
                }
            }
        }

        private static void TrackEnemy(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            orig(self);

            switch (self.gameObject.name)
            {
                case "Grub Mimic":
                case "Grub Mimic 1":
                case "Grub Mimic 2":
                case "Grub Mimic 3":
                case "Mage Knight":
                case "Mega Zombie Beam Miner (1)":
                case "Zombie Beam Miner Rematch":
                case "Giant Fly":
                    AddVanillaItem(self.gameObject.name, Utils.CurrentScene()??"");
                    break;
                default:
                    break;
            }
        }

        private static void AddItemTrackers(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            string name = self.gameObject.name;
            FsmState state;

            if (self.FsmName is "Shiny Control")
            {
                if (!FsmUtil.TryGetState(self, "Finish", out state)) return;
            }
            else if (name is "Heart Piece" || name is "Vessel Fragment")
            {
                if (!FsmUtil.TryGetState(self, "Get", out state)) return;
            }
            else if (self.FsmName is "Chest Control")
            {
                if (!FsmUtil.TryGetState(self, "Open", out state)) return;
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

            if (self.gameObject.scene.name is "Crossroads_ShamanTemple"
                && self.gameObject.name is "Geo Rock 2"
                && self.transform.parent != null)
            {
                self.geoRockData.id = "_Items/Geo Rock 2";
            }

            if (self.gameObject.scene.name is "Abyss_06_Core"
                && self.gameObject.name is "Geo Rock Abyss"
                && self.transform.parent != null)
            {
                self.geoRockData.id = "_Props/Geo Rock Abyss";
            }
        }

        private static void AddVanillaItem(string objectName, string scene)
        {
            clearedLocations.Add($"{objectName}-{scene}");
        }

        private static void AddSceneVisited(Scene from, Scene to)
        {
            ScenesVisited.Add(to.name);
        }
    }
}