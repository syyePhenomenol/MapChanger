using UnityEngine;

namespace MapChanger.Input;

public class ModEnabledInput : GlobalHotkeyInput
{
    internal ModEnabledInput()
        : base("Enable / Disable Map Mod", "MapMod", KeyCode.M)
    {
        Instance = this;
    }

    public static ModEnabledInput Instance { get; private set; }

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
        ModeManager.ToggleModEnabled();
    }
}
