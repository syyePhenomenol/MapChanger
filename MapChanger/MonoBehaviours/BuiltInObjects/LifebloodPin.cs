using System.Collections.Generic;

namespace MapChanger.MonoBehaviours
{
    internal class LifebloodPin : VanillaPin
    {
        private protected override string HasPinBoolName => "hasPinCocoon";

        private protected override string SceneListName => "scenesEncounteredCocoon";
    }
}
