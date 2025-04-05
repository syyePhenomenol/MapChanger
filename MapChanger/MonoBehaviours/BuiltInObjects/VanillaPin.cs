using System.Collections.Generic;

namespace MapChanger.MonoBehaviours;

public abstract class VanillaPin : MapObject
{
    private protected abstract string HasPinBoolName { get; }
    private protected abstract string SceneListName { get; }

    public override void Initialize()
    {
        ActiveModifiers.Add(VanillaPinsEnabled);
        ActiveModifiers.Add(HasEncounteredLocation);

        MapObjectUpdater.Add(this);
    }

    private bool VanillaPinsEnabled()
    {
        return !Settings.MapModEnabled() || (Settings.CurrentMode().VanillaPins ?? true);
    }

    private bool HasEncounteredLocation()
    {
        return (
                PlayerData.instance.GetBool(HasPinBoolName)
                && PlayerData.instance.GetVariable<List<string>>(SceneListName).Contains(transform.parent.name)
            ) || (Settings.CurrentMode().VanillaPins ?? false);
    }
}
