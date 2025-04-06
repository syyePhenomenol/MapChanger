using UnityEngine;

namespace MapChanger.Input;

public class DebugModKeyInputSource(string name) : KeyInputSource
{
    private KeyCode _key = KeyCode.None;

    public override KeyCode Key => _key;

    internal bool UpdateKeyBind()
    {
        if (DebugModInterop.TryGetBinding(name, out var binding) && _key != binding)
        {
            _key = binding;
            return true;
        }

        return false;
    }
}
