using Satchel.BetterMenus;

namespace MapChanger;

internal static class ModMenu
{
    internal static MenuScreen GetMenuScreen(MenuScreen modListMenu)
    {
        Menu menu =
            new(
                "Map Mod".L(),
                [
                    new HorizontalOption(
                        "Show Pause Menu".L(),
                        string.Empty,
                        ["Off".L(), "On".L()],
                        i => MapChangerMod.GS.ToggleShowPauseMenu(),
                        () => MapChangerMod.GS.ShowPauseMenu ? 1 : 0
                    ),
                ]
            );

        return menu.GetMenuScreen(modListMenu);
    }
}
