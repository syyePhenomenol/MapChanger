using System.Collections.Generic;
using VanillaMapMod.UI;

namespace MapChanger.UI;

internal class ModDisabledMenu : PauseMenuLayout
{
    public ModDisabledMenu()
        : base(nameof(MapChangerMod), nameof(ModDisabledMenu)) { }

    protected override bool ActiveCondition()
    {
        return base.ActiveCondition() && !MapChangerMod.IsEnabled();
    }

    protected override PauseMenuTitle GetTitle()
    {
        return new ModDisabledTitle();
    }

    protected override IEnumerable<MainButton> GetMainButtons()
    {
        return [new ModDisabledButton()];
    }

    protected override IEnumerable<ExtraButtonGrid> GetExtraButtonGrids()
    {
        return [];
    }
}
