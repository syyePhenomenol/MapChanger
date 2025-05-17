using MagicUI.Core;

namespace MapChanger.UI;

public abstract class UILayout : LayoutRoot
{
    public UILayout(string mod, string name)
        : base(true, $"{mod} {name}")
    {
        Mod = mod;
        VisibilityCondition = ActiveCondition;
    }

    public string Mod { get; private set; }

    public abstract void Update();

    public new virtual void Destroy()
    {
        base.Destroy();
    }

    protected abstract bool ActiveCondition();
}
