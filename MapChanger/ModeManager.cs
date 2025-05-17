using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using MapChanger.Map;
using Modding;
using UnityEngine;

namespace MapChanger;

public class ModeManager : HookModule
{
    private static readonly List<MapMode> _modes = [];
    private static int _modeIndex = 0;

    // When the map mod is enabled/disabled, or when the mode is changed
    public static event Action OnModeChanged;

    public override void OnEnterGame()
    {
        // Check if the mode can be loaded from a previously saved Settings
        for (var i = 0; i < _modes.Count; i++)
        {
            if (_modes[i].ModeKey == (MapChangerMod.LS.CurrentMod, MapChangerMod.LS.CurrentModeName))
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

        MapChangerMod.LS.SetCurrentMode(CurrentMode());
        MapChangerMod.Instance.LogDebug($"Mode initialized to {CurrentMode().ModeKey}");
    }

    public override void OnQuitToMenu()
    {
        _modes.Clear();
    }

    public static MapMode CurrentMode()
    {
        if (!HasModes())
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

    public static void ToggleModEnabled()
    {
        if (!HasModes())
        {
            return;
        }

        MapChangerMod.LS.ToggleEnabled();
        MapChangerMod.Instance.LogDebug($"Map mod toggled to {(MapChangerMod.LS.Enabled ? "Enabled" : "Disabled")}");
        UIManager.instance.checkpointSprite.Show();
        UIManager.instance.checkpointSprite.Hide();
        ModeChanged();
    }

    public static void ToggleMode()
    {
        if (!HasModes() || !MapChangerMod.LS.Enabled)
        {
            return;
        }

        _modeIndex = (_modeIndex + 1) % _modes.Count;
        MapChangerMod.LS.SetCurrentMode(CurrentMode());
        MapChangerMod.Instance.LogDebug($"Mode set to {CurrentMode().ModeKey}");
        ModeChanged();
    }

    private static void ModeChanged()
    {
        try
        {
            OnModeChanged?.Invoke();
        }
        catch (Exception e)
        {
            MapChangerMod.Instance.LogError(e);
        }

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
