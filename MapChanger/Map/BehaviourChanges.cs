using System;
using System.Linq;
using GlobalEnums;
using HutongGames.PlayMaker;
using MapChanger.MonoBehaviours;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                if (Settings.CurrentMode().VanillaPins ?? true)
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

        public override void OnEnterGame()
        {
            On.PlayMakerFSM.Start += ModifyFsms;

            On.HutongGames.PlayMaker.FsmState.OnEnter += SetQuickMapZone;
            On.GameManager.GetCurrentMapZone += OverrideGetCurrentMapZone;

            On.RoughMapRoom.OnEnable += StoreRoughMapCopy;
            
            On.MapNextAreaDisplay.OnEnable += NextAreaOverride;
            On.GameMap.SetupMapMarkers += SetupMarkersOverride;
            On.GameMap.DisableMarkers += DisableMarkersOverride;

            On.DeactivateIfPlayerdataFalse.Start += ControlMarkersUI;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += UpdateSceneMapZone;
            On.GameMap.PositionCompass += HideCompassInNonMappedScene;

            On.GameMap.Update += ZoomFasterOnKeyboard;
            On.GameMap.Update += OverridePanning;
            On.GameManager.UpdateGameMap += DisableUpdatedMapPrompt;

            Events.OnWorldMapInternal += IncreasePanningRange;
        }

        public override void OnQuitToMenu()
        {
            On.PlayMakerFSM.Start -= ModifyFsms;

            On.HutongGames.PlayMaker.FsmState.OnEnter -= SetQuickMapZone;
            On.GameManager.GetCurrentMapZone -= OverrideGetCurrentMapZone;

            On.RoughMapRoom.OnEnable -= StoreRoughMapCopy;

            On.MapNextAreaDisplay.OnEnable -= NextAreaOverride;
            On.GameMap.SetupMapMarkers -= SetupMarkersOverride;
            On.GameMap.DisableMarkers -= DisableMarkersOverride;

            On.DeactivateIfPlayerdataFalse.Start -= ControlMarkersUI;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= UpdateSceneMapZone;
            On.GameMap.PositionCompass -= HideCompassInNonMappedScene;

            On.GameMap.Update -= ZoomFasterOnKeyboard;
            On.GameMap.Update -= OverridePanning;
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
                if (rmc == null)
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

            self.gameObject.Child(MAP_MARKERS).SetActive(PlayerData.instance.GetBool(VariableOverrides.MARKERS_PREFIX + "hasMarker"));
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

        private static void ControlMarkersUI(On.DeactivateIfPlayerdataFalse.orig_Start orig, DeactivateIfPlayerdataFalse self)
        {
            orig(self);

            if (self.boolName is "hasMarker")
            {
                self.boolName = VariableOverrides.MARKERS_PREFIX + "hasMarker";
            }
        }

        // Cached map zone for the current scene.
        private static MapZone sceneMapZone;

        private static void UpdateSceneMapZone(Scene from, Scene to)
        {
            sceneMapZone = Finder.GetMapZone(GameManager.GetBaseSceneName(to.name));
            // MapChangerMod.Instance.LogDebug($"Updated sceneMapZone to {sceneMapZone} in scene {to.name}");
        }

        // currentMapZone is a local variable assigned during orig, so this is the most convenient way to override it
        private static bool overrideMapZone = false;

        private static void HideCompassInNonMappedScene(On.GameMap.orig_PositionCompass orig, GameMap self, bool posShade)
        {
            // MapChangerMod.Instance.LogDebug($"HideCompassInNonMappedScene");

            if (Settings.MapModEnabled() && sceneMapZone is not MapZone.NONE)
            {
                self.doorMapZone = sceneMapZone.ToString();
                overrideMapZone = true;
                // MapChangerMod.Instance.LogDebug($"Set map zone override to TRUE as {sceneMapZone}. Also set door map zone override");
            }

            orig(self, posShade);

            overrideMapZone = false;

            if (!Finder.IsMappedScene(Utils.CurrentScene()))
            {
                // MapChangerMod.Instance.LogDebug($"Disabled compass");
                self.compassIcon.SetActive(false);
                ReflectionHelper.SetField(self, "displayingCompass", false);
            }

            // MapChangerMod.Instance.LogDebug($"~HideCompassInNonMappedScene");
        }

        private static string OverrideGetCurrentMapZone(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
        {
            if (overrideMapZone)
            {
                // MapChangerMod.Instance.LogDebug($"Overwrote GetCurrentMapZone as {sceneMapZone}");

                return sceneMapZone.ToString();
            }

            // MapChangerMod.Instance.LogDebug($"Did NOT override GetCurrentMapZone as {sceneMapZone}");

            return orig(self);
        }

        private static void SetQuickMapZone(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, FsmState self)
        {
            if (Settings.MapModEnabled()
                && self.Name is "Check Area"
                && self.Fsm.GameObjectName is "Quick Map"
                && (self.Fsm.Variables.StringVariables.Where(var => var.Name is "Map Zone").FirstOrDefault() is FsmString mapZoneString)
                && sceneMapZone is not MapZone.NONE)
            {
                // MapChangerMod.Instance.LogDebug($"Overwrote Map Zone as {sceneMapZone}");
                mapZoneString.Value = sceneMapZone.ToString();
            }

            orig(self);
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

        private static void OverridePanning(On.GameMap.orig_Update orig, GameMap self)
        {
            bool canPanOrig = self.canPan;

            if (MapPanner.IsPanning)
            {
                self.canPan = false;
            }

            orig(self);

            self.canPan = canPanOrig;
        }

        /// <summary>
        /// QoL improvement which prevents the "Map Updated" prompt from occurring in most cases.
        /// If the game is saved while the mod is enabled, scenesMapped doesn't get updated. This is intentional.
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
