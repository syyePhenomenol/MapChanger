using System.Collections;

namespace MapChanger.MonoBehaviours;

public interface IPeriodicUpdater
{
    float UpdateWaitSeconds { get; }

    IEnumerator PeriodicUpdate();
}
