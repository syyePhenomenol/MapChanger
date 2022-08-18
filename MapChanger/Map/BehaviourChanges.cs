using System;
using GlobalEnums;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;
using Vasi;

namespace MapChanger.Map
{
    /// <summary>
    /// Many changes to the map behaviour with respect to removing buggy base game behaviour,
    /// getting toggles to work, and QoL improvements.
    /// </summary>
    internal class BehaviourChanges : HookModule
    {
        /// <summary>
        /// Displays the map key based on the current MapChanger setting.
        /// </summary>
        private class MapKeyUpCheck : FsmStateAction
        {
            public override void OnEnter()
            {
                if (!Settings.MapModEnabled() || Settings.CurrentMode().VanillaPins != OverrideType.ForceOff)
                {
                    PlayMakerFSM.BroadcastEvent("NEW MAP KEY ADDED");
                    MapKey?.gameObject.LocateMyFSM("Control")?.SendEvent("MAP KEY UP");
                }
                Finish();
            }
        }

        /// <summary>
        /// A copy of the rough map sprite in order to reset mapped rooms later.
        /// </summary>
        private class RoughMapCopy : MonoBehaviour
        {
            internal Sprite RoughMap { get; private set; }

            internal void AddSprite(Sprite sprite)
            {
                RoughMap = sprite;
            }
        }

        private const string MAP_MARKERS = "Map Markers";

        internal static Transform MapKey => GameCameras.instance.hudCamera.transform
            .FindChildInHierarchy("Inventory")?.FindChildInHierarchy("Map Key");
        internal static Transform MarkerAction => GameCameras.instance.hudCamera.transform
            .FindChildInHierarchy("Inventory")?.FindChildInHierarchy("Map")?
            .FindChildInHierarchy("World Map")?.FindChildInHierarchy("Map Marker Action");

        public override void OnEnterGame()
        {
            On.PlayMakerFSM.Start += ModifyFsms;
            On.RoughMapRoom.OnEnable += StoreRoughMapCopy;

            On.MapNextAreaDisplay.OnEnable += NextAreaOverride;
            On.GameMap.SetupMapMarkers += SetupMarkersOverride;
            On.GameMap.DisableMarkers += DisableMarkersOverride;

            On.GameMap.PositionCompass += HideCompassInNonMappedScene;
            On.GameMap.GetDoorMapZone += DoorMapZoneOverride;
            On.GameManager.GetCurrentMapZone += CurrentMapZoneOverride;

            On.GameMap.Update += ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap += DisableUpdatedMapPrompt;

            Events.OnWorldMapInternal += IncreasePanningRange;
        }

        public override void OnQuitToMenu()
        {
            On.PlayMakerFSM.Start -= ModifyFsms;
            On.RoughMapRoom.OnEnable -= StoreRoughMapCopy;

            On.MapNextAreaDisplay.OnEnable -= NextAreaOverride;
            On.GameMap.SetupMapMarkers -= SetupMarkersOverride;
            On.GameMap.DisableMarkers -= DisableMarkersOverride;

            On.GameMap.PositionCompass -= HideCompassInNonMappedScene;
            On.GameMap.GetDoorMapZone -= DoorMapZoneOverride;
            On.GameManager.GetCurrentMapZone -= CurrentMapZoneOverride;

            On.GameMap.Update -= ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap -= DisableUpdatedMapPrompt;

            Events.OnWorldMapInternal -= IncreasePanningRange;
        }

        private void ModifyFsms(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            try
            {
                if (self.gameObject.name == "World Map" && self.FsmName == "UI Control"
                    && self.TryGetState("Zoomed In", out FsmState state))
                {
                    state.RemoveAction(1);
                    state.InsertAction(1, new MapKeyUpCheck());
                }
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }

            orig(self);
        }

        private static void StoreRoughMapCopy(On.RoughMapRoom.orig_OnEnable orig, RoughMapRoom self)
        {
            if (self.GetComponent<SpriteRenderer>() is SpriteRenderer sr)
            {
                RoughMapCopy rmc = self.GetComponent<RoughMapCopy>();
                if (rmc is null)
                {
                    rmc = self.gameObject.AddComponent<RoughMapCopy>();
                    rmc.AddSprite(sr.sprite);
                }
                else
                {
                    sr.sprite = rmc.RoughMap;
                    self.fullSpriteDisplayed = false;
                }
            }

            orig(self);
        }

        /// <summary>
        /// Normally, this sets whether the next area object is active or not. However, MapChanger does it
        /// through the NextArea MapObject.
        /// </summary>
        private void NextAreaOverride(On.MapNextAreaDisplay.orig_OnEnable orig, MapNextAreaDisplay self)
        {
            
        }

        private static void SetupMarkersOverride(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);

            self.gameObject.Child(MAP_MARKERS).SetActive(PlayerData.instance.GetBool("hasMarker"));
        }

        /// <summary>
        /// The game doesn't normally set the MapMarkers gameObject to false when the map is closed, which
        /// leads to some visual bugs.
        /// </summary>
        private static void DisableMarkersOverride(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            orig(self);

            self.gameObject.Child(MAP_MARKERS).SetActive(false);
        }

        private static void HideCompassInNonMappedScene(On.GameMap.orig_PositionCompass orig, GameMap self, bool posShade)
        {
            // Not sure if necessary, but it might fix some NREs
            self.doorMapZone = self.GetDoorMapZone();

            orig(self, posShade);

            if (!Finder.IsMappedScene(Utils.CurrentScene()))
            {
                self.compassIcon.SetActive(false);
                ReflectionHelper.SetField(self, "displayingCompass", false);
            }
        }

        /// <summary>
        /// This fixes loading the Quick Map in some of the non-mapped areas (like Ancestral Mound).
        /// </summary>
        private static string DoorMapZoneOverride(On.GameMap.orig_GetDoorMapZone orig, GameMap self)
        {
            if (!Settings.MapModEnabled()) return orig(self);

            MapZone mapZone = Finder.GetCurrentMapZone();
            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }
            return orig(self);
        }

        private static string CurrentMapZoneOverride(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
        {
            if (!Settings.MapModEnabled()) return orig(self);

            // Modifying the MapZone in colosseum results in a softlock
            if (Utils.CurrentScene() is "Room_Colosseum_Bronze" or "Room_Colosseum_Silver" or "Room_Colosseum_Gold") return orig(self);

            MapZone mapZone = Finder.GetCurrentMapZone();
            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }
            return orig(self);
        }

        private static void ZoomFasterOnKeyboard(On.GameMap.orig_Update orig, GameMap self)
        {
            if (Settings.MapModEnabled()
                && self.canPan
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (InputHandler.Instance.inputActions.down.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y + self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.up.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y - self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.left.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x + self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.right.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x - self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
            }

            orig(self);
        }

        /// <summary>
        /// QoL improvement which prevents the "Map Updated" prompt from occurring in most cases.
        /// </summary>
        private static bool DisableUpdatedMapPrompt(On.GameManager.orig_UpdateGameMap orig, GameManager self)
        {
            if (Settings.MapModEnabled())
            {
                return false;
            }

            return orig(self);
        }

        /// <summary>
        /// Increases the range of panning so that objects on the map can be selected properly.
        /// </summary>
        private static void IncreasePanningRange(GameMap gameMap)
        {
            gameMap.panMinX = -29f;
            gameMap.panMaxX = 26f;
            gameMap.panMinY = -25f;
            gameMap.panMaxY = 24f;
        }
    }
}
