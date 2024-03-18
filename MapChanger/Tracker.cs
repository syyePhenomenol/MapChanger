using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using MapChanger.Tracking;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;

namespace MapChanger
{
    public class Tracker : HookModule
    {
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
                ObtainedSceneData.Add((grd.id, grd.sceneName));
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

                ObtainedSceneData.Add((name, scene));

                MapChangerMod.Instance.LogDebug("Item picked up");
                MapChangerMod.Instance.LogDebug(" Name: " + name);
                MapChangerMod.Instance.LogDebug(" Scene: " + scene);

                Finish();
            }
        }

        private static Dictionary<string, TrackingItem> trackingItems = new();

        internal static HashSet<(string, string)> ObtainedSceneData = new();

        public static HashSet<string> ScenesVisited = new();

        internal static void Load()
        {
            trackingItems = JsonUtil.Deserialize<Dictionary<string, TrackingItem>>("MapChanger.Resources.trackingItems.json");
        }

        internal static void VerifyTrackingDefs()
        {
            foreach (string name in trackingItems.Keys)
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
            // Hotfix for handling Level 3 Flames when Brumm flame is taken

            if (trackingItems.TryGetValue(name, out TrackingItem ti))
            {
                return ti.Has();
            }

            return false;
        }

        public static bool HasVisitedScene(string scene)
        {
            return ScenesVisited.Contains(scene);
        }

        private static void GetPreviouslyObtainedItems()
        {
            ObtainedSceneData = new();

            foreach (GeoRockData grd in GameManager.instance.sceneData.geoRocks)
            {
                if (grd.hitsLeft <= 0)
                {
                    ObtainedSceneData.Add((grd.id, grd.sceneName));
                }
            }

            HashSet<(string, string)> persistentBoolItems = new(trackingItems.Values.Where(i => i is SdTrackingItem)
                .Select(i => ((SdTrackingItem)i).SceneDataPair));

            persistentBoolItems.UnionWith(trackingItems.Values.Where(i => i is MultiTrackingItem)
                .SelectMany(i => ((MultiTrackingItem)i).GetSdTrackingItemPairs()));

            foreach (var pbd in GameManager.instance.sceneData.persistentBoolItems)
            {
                var pair = (pbd.id, pbd.sceneName);
                if (persistentBoolItems.Contains(pair) && pbd.activated)
                {
                    ObtainedSceneData.Add(pair);
                }
            }
        }

        private static void TrackEnemy(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            orig(self);

            var scene = Utils.CurrentScene();

            switch (self.gameObject.name)
            {
                case "Grub Mimic":
                case "Grub Mimic 1":
                case "Grub Mimic 2":
                case "Grub Mimic 3":
                case "Mega Zombie Beam Miner (1)":
                case "Zombie Beam Miner Rematch":
                    ObtainedSceneData.Add((self.gameObject.name, scene??""));
                    break;
                case "Mage Knight":
                    if (scene is "Ruins1_23")
                    {
                        ObtainedSceneData.Add(("Battle Scene v2", scene));
                    }
                    else if (scene is "Ruins1_31b")
                    {
                        ObtainedSceneData.Add(("Battle Scene v2", scene));
                    }
                    break;
                case "Giant Fly":
                    if (scene is "Crossroads_04")
                    {
                        ObtainedSceneData.Add(("Battle Scene", scene));
                    }
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

        private static void AddSceneVisited(Scene from, Scene to)
        {
            ScenesVisited.Add(to.name);
        }
    }
}