using UnityEngine;

namespace MapChanger.Input;

public class ToggleModeInput : GlobalHotkeyInput
{
    internal ToggleModeInput()
        : base("Toggle Map Mode", "Map Mods", KeyCode.T)
    {
        Instance = this;
    }

    public static ToggleModeInput Instance { get; private set; }

    public override bool UseCondition()
    {
        return true;
    }

    public override bool ActiveCondition()
    {
        return true;
    }

    public override void DoAction()
    {
        Settings.ToggleMode();
    }
}
