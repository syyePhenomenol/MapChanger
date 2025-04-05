using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI;

/// <summary>
/// A wrapper for MagicUI's Button class.
/// </summary>
/// <param name="mod"></param>
/// <param name="name"></param>
public abstract class ButtonWrapper(string name, string mod) : MagicUIWrapper(name, mod)
{
    public Button Button { get; private set; }

    public sealed override void Initialize(LayoutRoot root)
    {
        Button = MakeButton(root);
        Button.Click += OnClickInternal;
        Button.OnHover += OnHover;
        Button.OnUnhover += OnUnhover;
    }

    private protected virtual void OnClickInternal(Button button)
    {
        OnClick();
        PauseMenu.Update();
    }

    private protected virtual void OnHover(Button button)
    {
        OnHover();
    }

    private protected virtual void OnUnhover(Button button)
    {
        OnUnhover();
    }

    protected abstract Button MakeButton(LayoutRoot root);

    protected virtual void OnClick() { }

    protected virtual void OnHover() { }

    protected virtual void OnUnhover() { }
}
