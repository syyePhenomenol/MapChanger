using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using MapChanger.Map;
using MapChanger.UI;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace MapChanger
{
    public class Settings : HookModule
    {
        [JsonProperty]
        private bool mapModEnabled = false;
        [JsonProperty]
        private string currentMod = "MapChangerMod";
        [JsonProperty]
        private string currentModeName = "Disabled";

        internal static Settings Instance { get; set; }

        public static event Action OnSettingChanged;

        private static List<MapMode> modes = new();

        private static int modeIndex = 0;

        public static bool MapModWasEnabled { get; private set; } = false;

        public override void OnEnterGame()
        {
            // Check if the mode can be loaded from a previously saved Settings
            for (int i = 0; i < modes.Count; i++)
            {
                if (modes[i].ModeKey == (Instance.currentMod, Instance.currentModeName))
                {
                    modeIndex = i;
                    MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey} from loaded Settings");
                    MapModWasEnabled = true;
                    return;
                }
            }

            // If a new save, initialize mode to the highest priority existing mode
            float highestPriority = float.PositiveInfinity;
            for (int i = 0; i < modes.Count; i++)
            {
                MapMode mode = modes[i];
                if (mode.InitializeToThis() && mode.Priority < highestPriority)
                {
                    modeIndex = i;
                    highestPriority = mode.Priority;
                }
            }

            MapChangerMod.Instance.LogDebug($"Mode initialized to {CurrentMode().ModeKey}");
        }

        public override void OnQuitToMenu()
        {
            modes = new();
            MapModWasEnabled = false;
        }

        public static bool HasModes()
        {
            return modes.Any();
        }

        public static void AddModes(MapMode[] modes)
        {
            foreach (MapMode mode in modes)
            {
                if (Settings.modes.Any(existingMode => existingMode.ModeKey == mode.ModeKey))
                {
                    MapChangerMod.Instance.LogWarn($"A mode with the same key has already been added! {mode.ModeKey}");
                    continue;
                }

                Settings.modes.Add(mode);
            }
        }

        public static bool MapModEnabled()
        {
            return Instance.mapModEnabled;
        }

        public static void ToggleModEnabled()
        {
            if (!modes.Any()) return;

            Instance.mapModEnabled = !Instance.mapModEnabled;

            UIManager.instance.checkpointSprite.Show();
            UIManager.instance.checkpointSprite.Hide();

            SettingChanged();
        }

        public static MapMode CurrentMode()
        {
            if (!modes.Any())
            {
                return new();
            }
            if (modeIndex >= modes.Count)
            {
                MapChangerMod.Instance.LogWarn("Mode index overflow");
                modeIndex = 0;
            }
            return modes[modeIndex];
        }

        public static void ToggleMode()
        {
            if (!modes.Any() || !Instance.mapModEnabled) return;

            modeIndex = (modeIndex + 1) % modes.Count;
            MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey}");
            SettingChanged();
        }

        private static void SettingChanged()
        {
            MapModWasEnabled = true;

            Instance.currentMod = CurrentMode().Mod;
            Instance.currentModeName = CurrentMode().ModeName;

            PauseMenu.Update();
            MapUILayerUpdater.Update();
            MapObjectUpdater.Update();

            try { OnSettingChanged?.Invoke(); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            if (States.WorldMapOpen)
            {
                GameManager.instance.StartCoroutine(CloseAndOpenWorldMap());
            }

            if (States.QuickMapOpen)
            {
                SetQuickMapButton(false);
            }
        }

        private static IEnumerator CloseAndOpenWorldMap()
        {
            SetQuickMapButton(true);
            
            yield return new WaitForSeconds(0.3f);

            if (PlayerData.instance.GetBool(VariableOverrides.MAP_PREFIX + VariableOverrides.HAS_MAP))
            {
                GameManager.instance.inventoryFSM.SendEvent("OPEN INVENTORY MAP");
            }
        }

        private static void SetQuickMapButton(bool value)
        {
            InputHandler.Instance.inputActions.quickMap.CommitWithState(value, ReflectionHelper.GetField<OneAxisInputControl, ulong>(InputHandler.Instance.inputActions.quickMap, "pendingTick") + 1, 0);
        }
    }
}
