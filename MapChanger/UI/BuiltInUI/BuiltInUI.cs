using MapChanger.UI;

namespace MapChanger;

public class BuiltInUI : HookModule
{
    private static ModDisabledMenu _modDisabledMenu;

    public override void OnEnterGame()
    {
        _modDisabledMenu = new();
    }

    public override void OnQuitToMenu()
    {
        _modDisabledMenu?.Destroy();
        _modDisabledMenu = null;
    }
}
