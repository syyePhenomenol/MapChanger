namespace MapChanger.Input;

public abstract class AbstractInputSource
{
    public abstract bool GetPressed();
    public abstract bool GetReleased();
    public abstract string GetBindingsText();
}
