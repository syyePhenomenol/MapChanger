using MagicUI.Core;

namespace MapChanger.UI;

public abstract class WorldMapElement<T> : UIElementWrapper<WorldMapLayout, T>
    where T : ArrangableElement { }
