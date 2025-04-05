using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapChanger.MonoBehaviours;

public abstract class MapInputListener : MapObject
{
    public void Initialize(IEnumerable<MapInput> inputs)
    {
        base.Initialize();
        DontDestroyOnLoad(this);
        ActiveModifiers.Add(() => States.WorldMapOpen);
        MapObjectUpdater.Add(this);

        MapInputs = new([.. inputs]);
    }

    public ReadOnlyCollection<MapInput> MapInputs { get; private set; }

    public override void BeforeMainUpdate()
    {
        foreach (var input in MapInputs)
        {
            input.OnMainUpdate();
        }
    }

    public void Update()
    {
        foreach (var input in MapInputs)
        {
            input.Listen();
        }
    }

    public virtual bool ActiveCondition()
    {
        return Settings.MapModEnabled() && States.WorldMapOpen;
    }
}
