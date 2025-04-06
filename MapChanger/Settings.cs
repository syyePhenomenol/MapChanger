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

namespace MapChanger;

public class Settings : HookModule
{
    [JsonProperty]
    public bool Enabled { get; private set; } = false;

    [JsonProperty]
    public string CurrentMod { get; private set; } = nameof(MapChangerMod);

    [JsonProperty]
    public string CurrentModeName { get; private set; } = "Disabled";

    private static List<MapMode> _modes = [];

    private static int _modeIndex = 0;

    public static event Action OnSettingChanged;

    internal static Settings Instance { get; set; }

    public override void OnEnterGame()
    {
        // Check if the mode can be loaded from a previously saved Settings
        for (var i = 0; i < _modes.Count; i++)
        {
            if (_modes[i].ModeKey == (Instance.CurrentMod, Instance.CurrentModeName))
            {
                _modeIndex = i;
                MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey} from loaded Settings");
                return;
            }
        }

        // If a new save, initialize mode to the highest priority existing mode
        var highestPriority = float.PositiveInfinity;
        for (var i = 0; i < _modes.Count; i++)
        {
            var mode = _modes[i];
            if (mode.InitializeToThis() && mode.Priority < highestPriority)
            {
                _modeIndex = i;
                highestPriority = mode.Priority;
            }
        }

        MapChangerMod.Instance.LogDebug($"Mode initialized to {CurrentMode().ModeKey}");
    }

    public override void OnQuitToMenu()
    {
        _modes = [];
    }

    public static bool HasModes()
    {
        return _modes.Any();
    }

    public static void AddModes(IEnumerable<MapMode> modes)
    {
        foreach (var mode in modes)
        {
            if (_modes.Any(existingMode => existingMode.ModeKey == mode.ModeKey))
            {
                MapChangerMod.Instance.LogWarn($"A mode with the same key has already been added! {mode.ModeKey}");
                continue;
            }

            _modes.Add(mode);
        }
    }

    public static bool MapModEnabled()
    {
        return Instance.Enabled;
    }

    public static void ToggleModEnabled()
    {
        if (!_modes.Any())
            return;

        Instance.Enabled = !Instance.Enabled;

        UIManager.instance.checkpointSprite.Show();
        UIManager.instance.checkpointSprite.Hide();

        SettingChanged();
    }

    public static MapMode CurrentMode()
    {
        if (!_modes.Any())
        {
            return new();
        }

        if (_modeIndex >= _modes.Count)
        {
            MapChangerMod.Instance.LogWarn("Mode index overflow");
            _modeIndex = 0;
        }

        return _modes[_modeIndex];
    }

    public static void ToggleMode()
    {
        if (!_modes.Any() || !Instance.Enabled)
        {
            return;
        }

        _modeIndex = (_modeIndex + 1) % _modes.Count;
        MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey}");
        SettingChanged();
    }

    private static void SettingChanged()
    {
        Instance.CurrentMod = CurrentMode().Mod;
        Instance.CurrentModeName = CurrentMode().ModeName;

        PauseMenu.Update();
        MapUILayerUpdater.Update();
        MapObjectUpdater.Update();

        try
        {
            OnSettingChanged?.Invoke();
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

        if (States.WorldMapOpen)
        {
            _ = GameManager.instance.StartCoroutine(CloseAndOpenWorldMap());
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
        InputHandler.Instance.inputActions.quickMap.CommitWithState(
            value,
            ReflectionHelper.GetField<OneAxisInputControl, ulong>(
                InputHandler.Instance.inputActions.quickMap,
                "pendingTick"
            ) + 1,
            0
        );
    }
}
