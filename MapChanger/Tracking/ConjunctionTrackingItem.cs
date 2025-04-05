using System.Linq;

namespace MapChanger.Tracking;

public class ConjunctionTrackingItem : MultiTrackingItem
{
    public override bool Has()
    {
        return TrackingItems.All(i => i.Has());
    }
}
