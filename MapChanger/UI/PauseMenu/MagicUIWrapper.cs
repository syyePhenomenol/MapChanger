using MagicUI.Core;

namespace MapChanger.UI;

public abstract class MagicUIWrapper(string name, string mod)
{
    public string Name { get; } = name;
    public string Mod { get; } = mod;
    public string FullName => $"{Mod} {Name}";

    public abstract void Initialize(LayoutRoot root);

    public abstract void Update();
}
