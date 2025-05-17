using MagicUI.Core;

namespace MapChanger.UI;

/// <summary>
/// Base class wrapper for all MagicUI elements.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public abstract class UIElementWrapper<T1, T2>
    where T1 : UILayout
    where T2 : ArrangableElement
{
    public virtual string Name => $"{Layout.Mod} {GetType().Name}";
    public T1 Layout { get; private set; }
    public T2 Element { get; protected internal set; }

    public virtual void Initialize(T1 layout)
    {
        Layout = layout;
        Element = Build(layout);
    }

    public abstract void Update();

    public virtual void Destroy()
    {
        Element?.Destroy();
    }

    protected abstract T2 Build(T1 layout);
}
