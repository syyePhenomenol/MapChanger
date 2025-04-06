using UnityEngine;

namespace MapChanger.Input;

public class FixedKeyInputSource(KeyCode key) : AbstractInputSource
{
    public KeyCode Key { get; } = key;

    public override bool GetPressed()
    {
        return UnityEngine.Input.GetKeyDown(Key);
    }

    public override bool GetReleased()
    {
        return UnityEngine.Input.GetKeyUp(Key);
    }

    public override string GetBindingsText()
    {
        return Key.ToChar();
    }
}
