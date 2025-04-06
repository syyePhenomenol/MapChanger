using System;
using InControl;
using UnityEngine;

namespace MapChanger.Input;

public abstract class WorldMapInput(
    string name,
    string mod,
    Func<PlayerAction> getPlayerAction,
    float holdMilliseconds = 0
) : BindableInput(name, mod, new PlayerActionInputSource(getPlayerAction), holdMilliseconds)
{
    public override bool ActiveCondition()
    {
        return States.WorldMapOpen;
    }

    public override bool ModifierKeyCondition()
    {
        return !(UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.LeftControl));
    }

    public override string GetBindingsText()
    {
        if (CurrentInput is DebugModKeyInputSource)
        {
            return $"[{CurrentInput.GetBindingsText()}]";
        }

        return CurrentInput?.GetBindingsText() ?? "UNKNOWN";
    }
}
