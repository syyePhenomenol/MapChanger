using UnityEngine;

namespace MapChanger.Input;

public abstract class GlobalHotkeyInput(string name, string mod, KeyCode key)
    : BindableInput(name, $"{mod} (Ctrl+)", new FixedKeyInputSource(key))
{
    public override bool ModifierKeyCondition()
    {
        return UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
    }

    public override string GetBindingsText()
    {
        return $"[Ctrl + {CurrentInput?.GetBindingsText() ?? "UNKNOWN"}]";
    }
}
