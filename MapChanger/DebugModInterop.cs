using UnityEngine;

namespace MapChanger;

// DebugMod should only be used as a GUI for setting and getting custom binds. We do not make DebugMod directly do anything
// when the keybinds are pressed, since it does not support holding keys.
public static class DebugModInterop
{
    // To be called only ever once per different binding in Initialize().
    public static void AddBinding(string name, string mod)
    {
        DebugMod.DebugMod.AddActionToKeyBindList(() => { }, name, mod);
    }

    internal static bool TryGetBinding(string name, out KeyCode keyCode)
    {
        if (DebugMod.DebugMod.settings.binds.TryGetValue(name, out keyCode))
        {
            return true;
        }

        keyCode = KeyCode.None;
        return false;
    }
}
