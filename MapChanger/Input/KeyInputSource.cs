using UnityEngine;

namespace MapChanger.Input;

public abstract class KeyInputSource : AbstractInputSource
{
    public abstract KeyCode Key { get; }

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
