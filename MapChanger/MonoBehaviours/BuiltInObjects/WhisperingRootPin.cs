using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

internal class WhisperingRootPin : VanillaPin
{
    private protected override string HasPinBoolName => "hasPinDreamPlant";
    private protected override string SceneListName => "scenesEncounteredDreamPlant";

    public override void Initialize()
    {
        base.Initialize();

        if (transform.parent.name == "Hive_02")
        {
            MapChangerMod.Instance.LogDebug("Fixing position of Hive_02 root pin");
            transform.localPosition = new Vector3(0.4f, -0.32f, -0.05f);
        }

        ActiveModifiers.Add(NotClearedWhisperingRoot);
    }

    private bool NotClearedWhisperingRoot()
    {
        return !PlayerData
            .instance.GetVariable<List<string>>("scenesEncounteredDreamPlantC")
            .Contains(transform.parent.name);
    }
}
