using UnityEngine;

namespace MapChanger;

// DebugMod should only be used as a GUI for setting and getting custom binds. We do not make DebugMod directly do anything
// when the keybinds are pressed, since it does not support holding keys.
internal static class DebugModInterop
{
    internal static void AddBinding(string name, string category)
    {
        DebugMod.DebugMod.AddActionToKeyBindList(() => { }, name, category);
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
